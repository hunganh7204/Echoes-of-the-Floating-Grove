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
        titleText.text = isVictory ? "VICTORY!" : "YOU LOSE";
        titleText.color = isVictory
    ? new Color32(46, 125, 50, 255)  
    : new Color32(198, 40, 40, 255);

        levelText.text = $"Level: {currentLevel}";
        scoreText.text = $"Score: {score}";
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