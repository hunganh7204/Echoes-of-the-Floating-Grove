using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Tải lại thiết lập âm lượng ở lần chơi trước
        LoadVolume();
    }

    // Hàm này được gọi trực tiếp từ SettingUI
    public void SetVolume(string parameterName, float sliderValue)
    {
        // Chuyển đổi giá trị Slider (0.0001 -> 1) sang Decibel (-80dB -> 0dB)
        // Dùng Mathf.Max để đảm bảo giá trị không bao giờ bằng 0 (Log10 của 0 sẽ báo lỗi)
        float decibel = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;

        audioMixer.SetFloat(parameterName, decibel);
    }

    public void LoadVolume()
    {
        // Đọc dữ liệu từ PlayerPrefs (đã được SettingUI lưu lại)
        if (PlayerPrefs.HasKey("MasterVol"))
            SetVolume("MasterVolume", PlayerPrefs.GetFloat("MasterVol"));

        if (PlayerPrefs.HasKey("BGMVol"))
            SetVolume("BGMVolume", PlayerPrefs.GetFloat("BGMVol"));

        if (PlayerPrefs.HasKey("SFXVol"))
            SetVolume("SFXVolume", PlayerPrefs.GetFloat("SFXVol"));
    }
}