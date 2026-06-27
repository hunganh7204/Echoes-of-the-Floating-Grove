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

    [SerializeField] private Button moveLeftBindButton;
    [SerializeField] private TextMeshProUGUI moveLeftBindText;

    [SerializeField] private Button moveRightBindButton;
    [SerializeField] private TextMeshProUGUI moveRightBindText;

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
        masterVolumeSlider.value = PlayerPrefs.HasKey("MasterVol") ? PlayerPrefs.GetFloat("MasterVol") : 1f;
        bgmVolumeSlider.value = PlayerPrefs.HasKey("BGMVol") ? PlayerPrefs.GetFloat("BGMVol") : 1f;
        sfxVolumeSlider.value = PlayerPrefs.HasKey("SFXVol") ? PlayerPrefs.GetFloat("SFXVol") : 1f;

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        closeButton.onClick.AddListener(ApplySettings);
        resetBindingsButton.onClick.AddListener(ResetToDefault);


        moveLeftBindButton.onClick.AddListener(() => StartRebinding("Move", moveLeftBindText, 1));
        moveRightBindButton.onClick.AddListener(() => StartRebinding("Move", moveRightBindText, 2));

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
        PlayerPrefs.SetFloat("MasterVol", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("BGMVol", bgmVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxVolumeSlider.value);
        PlayerPrefs.Save();

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

    public void StartRebinding(string actionName, TextMeshProUGUI bindingText, int bindingIndex = 0)
    {
        bindingText.text = "...";

        if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = false;

        InputManager.Instance.RebindAction(actionName, () =>
        {
            if (settingsCanvasGroup != null) settingsCanvasGroup.interactable = true;
            RefreshAllBindingTexts();
        }, bindingIndex);
    }

    public void ResetToDefault()
    {

        masterVolumeSlider.value = 1f;
        bgmVolumeSlider.value = 1f;
        sfxVolumeSlider.value = 1f;

        PlayerPrefs.DeleteKey("MasterVol");
        PlayerPrefs.DeleteKey("BGMVol");
        PlayerPrefs.DeleteKey("SFXVol");

        if (InputManager.Instance != null)
        {
            InputManager.Instance.ResetBindingsToDefault();
        }

        RefreshAllBindingTexts();
        PlayerPrefs.Save();
    }
}