using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        CheckSaveFile();
    }

    private void OnEnable()
    {
        CheckSaveFile();
    }

    private void CheckSaveFile()
    {
 
        PlayerData savedData = DataManager.Instance.LoadGame();

        if (savedData != null)
        {
            continueButton.interactable = true; 
        }
        else
        {
            continueButton.interactable = false; 
        }
    }

    private void OnNewGameClicked()
    {

        string saveFilePath = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        UIManager.Instance.StartNewGameFromMenu();
    }

    private void OnContinueClicked()
    {
        UIManager.Instance.ContinueGameFromMenu();
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.OpenSettings();
    }

    private void OnQuitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}