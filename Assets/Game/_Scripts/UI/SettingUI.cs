using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Keybinding UI")]
    // --- Các nút đã có ---
    [SerializeField] private Button interactBindButton;
    [SerializeField] private TextMeshProUGUI interactBindText;

    [SerializeField] private Button echoBindButton; // Dùng cho RecordEcho
    [SerializeField] private TextMeshProUGUI echoBindText;

    [SerializeField] private Button gravityBindButton;
    [SerializeField] private TextMeshProUGUI gravityBindText;

    [SerializeField] private Button moveLeftBindButton;
    [SerializeField] private TextMeshProUGUI moveLeftBindText;

    [SerializeField] private Button moveRightBindButton;
    [SerializeField] private TextMeshProUGUI moveRightBindText;

    // --- Các nút bổ sung thêm ---
    [SerializeField] private Button jumpBindButton;
    [SerializeField] private TextMeshProUGUI jumpBindText;

    [SerializeField] private Button attackBindButton;
    [SerializeField] private TextMeshProUGUI attackBindText;

    [SerializeField] private Button sprintBindButton;
    [SerializeField] private TextMeshProUGUI sprintBindText;

    [SerializeField] private Button replayEchoBindButton;
    [SerializeField] private TextMeshProUGUI replayEchoBindText;

    [SerializeField] private Button cancelEchoBindButton;
    [SerializeField] private TextMeshProUGUI cancelEchoBindText;

    [Header("References")]
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button resetBindingsButton;

    private void Start()
    {
        // Load cấu hình âm thanh
        masterVolumeSlider.value = PlayerPrefs.HasKey("MasterVol") ? PlayerPrefs.GetFloat("MasterVol") : 1f;
        bgmVolumeSlider.value = PlayerPrefs.HasKey("BGMVol") ? PlayerPrefs.GetFloat("BGMVol") : 1f;
        sfxVolumeSlider.value = PlayerPrefs.HasKey("SFXVol") ? PlayerPrefs.GetFloat("SFXVol") : 1f;

        // Gắn sự kiện âm thanh
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        closeButton.onClick.AddListener(ApplySettings);
        resetBindingsButton.onClick.AddListener(ResetToDefault);

        // Gắn sự kiện đổi phím (Move trái/phải dùng index 1 và 2)
        moveLeftBindButton.onClick.AddListener(() => StartRebinding("Move", moveLeftBindText, 1));
        moveRightBindButton.onClick.AddListener(() => StartRebinding("Move", moveRightBindText, 2));

        // Các phím còn lại dùng index 0 mặc định
        interactBindButton.onClick.AddListener(() => StartRebinding("Interact", interactBindText));
        echoBindButton.onClick.AddListener(() => StartRebinding("RecordEcho", echoBindText));
        gravityBindButton.onClick.AddListener(() => StartRebinding("Gravity", gravityBindText));
        jumpBindButton.onClick.AddListener(() => StartRebinding("Jump", jumpBindText));
        attackBindButton.onClick.AddListener(() => StartRebinding("Attack", attackBindText));
        sprintBindButton.onClick.AddListener(() => StartRebinding("Sprint", sprintBindText));
        replayEchoBindButton.onClick.AddListener(() => StartRebinding("ReplayEcho", replayEchoBindText));
        cancelEchoBindButton.onClick.AddListener(() => StartRebinding("CancelEcho", cancelEchoBindText));
    }

    private void OnEnable()
    {
        RefreshAllBindingTexts();
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume("MasterVolume", value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume("BGMVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume("SFXVolume", value);
    }

    public void ApplySettings()
    {
        // Lưu cấu hình âm thanh
        PlayerPrefs.SetFloat("MasterVol", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("BGMVol", bgmVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxVolumeSlider.value);
        PlayerPrefs.Save();

        // Lưu cấu hình phím
        if (InputManager.Instance != null) InputManager.Instance.SaveBindingsToData();

        if (UIManager.Instance != null) UIManager.Instance.CloseSettings();
    }

    private void RefreshAllBindingTexts()
    {
        if (InputManager.Instance != null)
        {
            if (moveLeftBindText != null) moveLeftBindText.text = InputManager.Instance.GetBindingName("Move", 1);
            if (moveRightBindText != null) moveRightBindText.text = InputManager.Instance.GetBindingName("Move", 2);

            if (interactBindText != null) interactBindText.text = InputManager.Instance.GetBindingName("Interact");
            if (echoBindText != null) echoBindText.text = InputManager.Instance.GetBindingName("RecordEcho");
            if (gravityBindText != null) gravityBindText.text = InputManager.Instance.GetBindingName("Gravity");

            if (jumpBindText != null) jumpBindText.text = InputManager.Instance.GetBindingName("Jump");
            if (attackBindText != null) attackBindText.text = InputManager.Instance.GetBindingName("Attack");
            if (sprintBindText != null) sprintBindText.text = InputManager.Instance.GetBindingName("Sprint");
            if (replayEchoBindText != null) replayEchoBindText.text = InputManager.Instance.GetBindingName("ReplayEcho");
            if (cancelEchoBindText != null) cancelEchoBindText.text = InputManager.Instance.GetBindingName("CancelEcho");
        }
    }

    // Tham số bindingIndex mặc định là 0 để tương thích với các phím thường
    public void StartRebinding(string actionName, TextMeshProUGUI bindingText, int bindingIndex = 0)
    {
        bindingText.text = "...";

        // Tắt tính năng tương tác của toàn bộ Panel Setting (Chặn click lung tung)
        if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = false;

        // Gọi sang InputManager truyền thêm bindingIndex
        InputManager.Instance.RebindAction(actionName, () =>
        {
            // Bật lại tương tác
            if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = true;

            // Bắt UI cập nhật lại toàn bộ chữ để hiển thị kết quả Hoán đổi (nếu có)
            RefreshAllBindingTexts();
        }, bindingIndex);
    }

    public void ResetToDefault()
    {
        // 1. Khôi phục Âm thanh về mức tối đa (1f)
        masterVolumeSlider.value = 1f;
        bgmVolumeSlider.value = 1f;
        sfxVolumeSlider.value = 1f;

        // Xóa dữ liệu âm thanh đã lưu
        PlayerPrefs.DeleteKey("MasterVol");
        PlayerPrefs.DeleteKey("BGMVol");
        PlayerPrefs.DeleteKey("SFXVol");

        // 2. Khôi phục Phím bấm qua InputManager
        if (InputManager.Instance != null)
        {
            InputManager.Instance.ResetBindingsToDefault();
        }

        // Cập nhật lại toàn bộ chữ cái trên giao diện về phím mặc định
        RefreshAllBindingTexts();

        PlayerPrefs.Save();
    }
}