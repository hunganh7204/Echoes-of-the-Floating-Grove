using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameResultUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("UI Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button loadCheckpointButton;

    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartClicked);
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        loadCheckpointButton.onClick.AddListener(OnLoadCheckpointClicked);
    }

    public void SetupPanel(bool isVictory, int currentLevel, int score)
    {
        titleText.text = isVictory ? "CHIẾN THẮNG!" : "BẠN ĐÃ GỤC NGÃ!";
        titleText.color = isVictory ? Color.green : Color.red;

        levelText.text = $"Màn chơi: {currentLevel}";
        scoreText.text = $"Điểm số: {score}";

        nextLevelButton.gameObject.SetActive(isVictory);
        loadCheckpointButton.gameObject.SetActive(!isVictory); 
    }

    private void OnRestartClicked()
    {
        UIManager.Instance.RestartLevel(false); 
    }

    private void OnLoadCheckpointClicked()
    {
        UIManager.Instance.RestartLevel(true); 
    }

    private void OnNextLevelClicked()
    {
        UIManager.Instance.StartGameplay();
        ScoreManager.Instance.ResetScore();

        int nextLevel = LevelGenerator.Instance.CurrentLevel + 1;
        LevelGenerator.Instance.LoadLevelAndSpawnPlayer(nextLevel);
    }

    private void OnMainMenuClicked()
    {
        UIManager.Instance.ReturnToMainMenu();
    }
}