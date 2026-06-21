using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI keyText;

    [Header("Position Settings")]
    [Tooltip("Khoảng cách đẩy UI nổi lên trên đỉnh đầu vật thể")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

    private Transform currentTarget;
    private Camera mainCam;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        mainCam = Camera.main;

        if (promptPanel != null) promptPanel.SetActive(false);
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            bool isGamePlaying = Time.timeScale > 0f;
            if (promptPanel.activeSelf != isGamePlaying)
            {
                promptPanel.SetActive(isGamePlaying);
            }
            if (isGamePlaying)
            {
                Vector3 screenPos = mainCam.WorldToScreenPoint(currentTarget.position + offset);
                promptPanel.transform.position = screenPos;
            }
        }
    }
    public void ShowPrompt(string actionDescription, Transform targetTransform)
    {
        currentTarget = targetTransform;

        string keyName = "F";
        if (InputManager.Instance != null)
        {
            keyName = InputManager.Instance.GetBindingName("Interact");
        }
        
        if (keyText != null)
        {
            keyText.text = keyName;
        }

        promptText.text = actionDescription;

        if (Time.timeScale > 0f)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(currentTarget.position + offset);
            promptPanel.transform.position = screenPos;
            promptPanel.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        currentTarget = null;
    }
}