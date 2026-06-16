using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public static PlayerStats Instance { get; private set; }

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 50;

    private int currentHealth;
    private int currentMana;

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
        //anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentMana = maxMana;

        if (PlayerHUDUI.Instance != null)
        {
            PlayerHUDUI.Instance.UpdateHealth(currentHealth, maxHealth);
            PlayerHUDUI.Instance.UpdateMana(currentMana, maxMana);
        }
    }

    // ==========================================
    // NH?N S�T TH��NG T? QU�I V?T
    // ==========================================
    public void TakeDamage(int damageAmount, Vector2 knockbackDirection = default)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (anim != null) anim.SetTrigger("3_Damage");
        if (PlayerHUDUI.Instance != null) PlayerHUDUI.Instance.UpdateHealth(currentHealth, maxHealth);
        Debug.Log($"<color=red>Player b? th��ng! M?t {damageAmount} m�u. C?n: {currentHealth}</color>");

        if (spriteRenderer != null) StartCoroutine(FlashRedEffect());

        if (knockbackDirection != default && rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0) Die();
    }

    // ==========================================
    // TI�U HAO MANA CHO K? N�NG
    // ==========================================
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            if (PlayerHUDUI.Instance != null) PlayerHUDUI.Instance.UpdateMana(currentMana, maxMana);
            Debug.Log($"<color=blue>�? d�ng {amount} Mana. C?n: {currentMana}</color>");
            return true;
        }
        Debug.Log("<color=yellow>Kh�ng �? Mana �? d�ng k? n�ng!</color>");
        return false;
    }

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
        Debug.Log("Player G?c Ng?!");
        gameObject.SetActive(false);
        UIManager.Instance.ShowGameResult(false);
    }

    public void ResetStats()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        spriteRenderer.color = Color.white;
        Debug.Log("Player: Đã khôi phục Máu và Mana.");
        if (PlayerHUDUI.Instance != null)
        {
            PlayerHUDUI.Instance.UpdateHealth(currentHealth, maxHealth);
            PlayerHUDUI.Instance.UpdateMana(currentMana, maxMana);
        }
    }
}