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

    [Header("Quit Confirmation Pop-up")]
    [SerializeField] private GameObject quitConfirmPanel;
    [SerializeField] private Button confirmQuitButton;    
    [SerializeField] private Button cancelQuitButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        loadCheckpointButton.onClick.AddListener(OnLoadCheckpointClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        if (quitButton != null) quitButton.onClick.AddListener(ShowQuitConfirm);
        if (confirmQuitButton != null) confirmQuitButton.onClick.AddListener(ExecuteQuit);
        if (cancelQuitButton != null) cancelQuitButton.onClick.AddListener(HideQuitConfirm);
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);
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
        UIManager.Instance.ContinueGameFromMenu();
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.OpenSettings();
    }

    //private void OnQuitClicked()
    //{
    //    UIManager.Instance.ReturnToMainMenu();
    //}

    private void OnDisable()
    {
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);
    }

    private void ShowQuitConfirm()
    {
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(true);
    }

    private void HideQuitConfirm()
    {
        if (quitConfirmPanel != null) quitConfirmPanel.SetActive(false);
    }

    private void ExecuteQuit()
    {
        HideQuitConfirm();
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ReturnToMainMenu();
        }
        else
        {
            Application.Quit();
        }
    }
}