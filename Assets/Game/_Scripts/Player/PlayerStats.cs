using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public static PlayerStats Instance { get; private set; }

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 50;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 15f; // Tốc độ hồi (điểm/giây)
    [SerializeField] private float staminaRegenDelay = 1f;

    private int currentHealth;
    private int currentMana;
    private float currentStamina;
    private float lastStaminaUseTime;

    public int CurrentHealth => currentHealth;
    public int CurrentMana => currentMana;
    public float CurrentStamina => currentStamina;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;

        UpdateAllUI();
    }

    private void Update()
    {
        if (currentStamina < maxStamina && Time.time >= lastStaminaUseTime + staminaRegenDelay)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;

            if (PlayerHUDUI.Instance != null)
                PlayerHUDUI.Instance.UpdateStamina(currentStamina, maxStamina);
        }
    }

//stamina
    public bool CanUseStamina(float amount)
    {
        return currentStamina >= amount;
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;

        lastStaminaUseTime = Time.time; // Cập nhật lại thời gian vừa dùng để tính Delay hồi

        if (PlayerHUDUI.Instance != null)
            PlayerHUDUI.Instance.UpdateStamina(currentStamina, maxStamina);
    }

//damage
    public void TakeDamage(int damageAmount, Vector2 knockbackDirection = default)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (anim != null) anim.SetTrigger("3_Damaged");
        if (PlayerHUDUI.Instance != null) PlayerHUDUI.Instance.UpdateHealth(currentHealth, maxHealth);

        if (spriteRenderer != null) StartCoroutine(FlashRedEffect());

        if (knockbackDirection != default && rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0) Die();
    }

//mana
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            if (PlayerHUDUI.Instance != null) PlayerHUDUI.Instance.UpdateMana(currentMana, maxMana);
            return true;
        }
        return false;
    }

//update stats
    private IEnumerator FlashRedEffect()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = original;
    }

    private void Die()
    {
        if (anim != null)
        {
            anim.SetTrigger("4_Death");
            anim.SetBool("isDeath", true);
        }
        gameObject.SetActive(false);
        UIManager.Instance.ShowGameResult(false);
    }

    public void ResetStats()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
        spriteRenderer.color = Color.white;
        UpdateAllUI();
    }
    public void LoadSavedStats(int savedHealth, int savedMana)
    {
        currentHealth = savedHealth;
        currentMana = savedMana;
        currentStamina = maxStamina; // Hồi full thể lực khi load Checkpoint
        UpdateAllUI();
    }
    private void UpdateAllUI()
    {
        if (PlayerHUDUI.Instance != null)
        {
            PlayerHUDUI.Instance.UpdateHealth(currentHealth, maxHealth);
            PlayerHUDUI.Instance.UpdateMana(currentMana, maxMana);
            PlayerHUDUI.Instance.UpdateStamina(currentStamina, maxStamina);
        }
    }
}