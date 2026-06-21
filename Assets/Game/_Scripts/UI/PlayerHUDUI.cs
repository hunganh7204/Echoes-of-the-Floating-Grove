using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDUI : MonoBehaviour
{
    public static PlayerHUDUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image manaFillImage;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void Update()
    {
        UpdateScore(ScoreManager.Instance.CurrentScore);
    }
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        if (manaFillImage != null)
            manaFillImage.fillAmount = (float)currentMana / maxMana;
    }

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        if (staminaFillImage != null)
            staminaFillImage.fillAmount = currentStamina / maxStamina;
    }

    public void UpdateScore(int currentScore)
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }
}