using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDUI : MonoBehaviour
{
    public static PlayerHUDUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image manaFillImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthFillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        manaFillImage.fillAmount = (float)currentMana / maxMana;
    }

    public void UpdateScore(int currentScore)
    {
        scoreText.text = currentScore.ToString();
    }
}