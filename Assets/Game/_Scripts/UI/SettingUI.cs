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

    [SerializeField] private Button interactBindButton;
    [SerializeField] private TextMeshProUGUI interactBindText;

    [SerializeField] private Button echoBindButton;
    [SerializeField] private TextMeshProUGUI echoBindText;

    [SerializeField] private Button gravityBindButton;
    [SerializeField] private TextMeshProUGUI gravityBindText;

    [SerializeField] private CanvasGroup settingsCanvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    private void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.HasKey("MasterVol") ? PlayerPrefs.GetFloat("MasterVol") : 1f;
        bgmVolumeSlider.value = PlayerPrefs.HasKey("BGMVol") ? PlayerPrefs.GetFloat("BGMVol") : 1f;
        sfxVolumeSlider.value = PlayerPrefs.HasKey("SFXVol") ? PlayerPrefs.GetFloat("SFXVol") : 1f;

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        closeButton.onClick.AddListener(ApplySettings);


        interactBindButton.onClick.AddListener(() => StartRebinding("Interact", interactBindText));
        echoBindButton.onClick.AddListener(() => StartRebinding("RecordEcho", echoBindText));
        gravityBindButton.onClick.AddListener(() => StartRebinding("Gravity", gravityBindText));
    }

    private void OnEnable()
    {
        RefreshAllBindingTexts();
    }

    public void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("MasterVolume", value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("BGMVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("SFXVolume", value);
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("MasterVol", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("BGMVol", bgmVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxVolumeSlider.value);
        PlayerPrefs.Save();

        InputManager.Instance.SaveBindingsToData();

        UIManager.Instance.CloseSettings();
    }

    private void RefreshAllBindingTexts()
    {
        if (InputManager.Instance != null)
        {
            if (interactBindText != null)
                interactBindText.text = InputManager.Instance.GetBindingName("Interact");

            if (echoBindText != null)
                echoBindText.text = InputManager.Instance.GetBindingName("RecordEcho");

            if (gravityBindText != null)
                gravityBindText.text = InputManager.Instance.GetBindingName("Gravity");
        }
    }

    public void StartRebinding(string actionName, TextMeshProUGUI bindingText)
    {
        bindingText.text = "...";

        // Tắt tính năng tương tác của toàn bộ Panel Setting (Chặn click lung tung)
        if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = false;

        // Gọi sang InputManager
        InputManager.Instance.RebindAction(actionName, () =>
        {
            // Bật lại tương tác
            if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = true;

            // Bắt UI cập nhật lại toàn bộ chữ để hiển thị kết quả Hoán đổi (nếu có)
            RefreshAllBindingTexts();
        });
    }
}
