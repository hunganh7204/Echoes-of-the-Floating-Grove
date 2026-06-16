using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button loadCheckpointButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        loadCheckpointButton.onClick.AddListener(OnLoadCheckpointClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnResumeClicked()
    {
        UIManager.Instance.StartGameplay();
    }

    private void OnRestartClicked()
    {
        UIManager.Instance.RestartLevel(false);
    }

    private void OnLoadCheckpointClicked()
    {
        UIManager.Instance.RestartLevel(true);
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.OpenSettings();
    }

    private void OnQuitClicked()
    {
        UIManager.Instance.ReturnToMainMenu();
    }
}