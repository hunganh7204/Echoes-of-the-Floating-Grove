using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        if (PlayerHUDUI.Instance != null)
        {
            PlayerHUDUI.Instance.UpdateScore(CurrentScore);
        }
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }
}