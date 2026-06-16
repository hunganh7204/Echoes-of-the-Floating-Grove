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
        // Gắn sự kiện (Listener) cho các nút
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        CheckSaveFile();
    }

    private void OnEnable()
    {
        // Cập nhật lại nút Continue mỗi khi Menu này được bật lên
        CheckSaveFile();
    }

    private void CheckSaveFile()
    {
        // Gọi DataManager xem có file save tiến trình không
        PlayerData savedData = DataManager.Instance.LoadGame();

        if (savedData != null)
        {
            continueButton.interactable = true; // Sáng nút Tiếp tục
        }
        else
        {
            continueButton.interactable = false; // Bấm mờ nút
        }
    }

    private void OnNewGameClicked()
    {
        // Xóa file save cũ nếu có để tạo tiến trình mới
        string saveFilePath = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("UI: Đã xóa tiến trình cũ để bắt đầu Game mới.");
        }

        // Báo cho UIManager chạy luồng Reset toàn bộ và sinh màn 1
        UIManager.Instance.StartNewGameFromMenu();
    }

    private void OnContinueClicked()
    {
        // Báo cho UIManager chạy luồng Load Checkpoint (sẽ tự động đọc data và xếp map)
        UIManager.Instance.ContinueGameFromMenu();
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.OpenSettings();
    }

    private void OnQuitClicked()
    {
        Debug.Log("UI: Thoát game!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}