using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject chatPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText; // Hiển thị nội dung câu nói (Đổi từ chatHistoryText)
    public TMP_InputField playerInputField;
    public Button sendButton;
    public Button closeButton;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.03f; // Tốc độ gõ chữ (càng nhỏ càng nhanh)
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        sendButton.onClick.AddListener(OnSubmitButtonPressed);
        closeButton.onClick.AddListener(CloseChat);

        // Nhấn Enter để gửi
        playerInputField.onSubmit.AddListener(delegate { OnSubmitButtonPressed(); });

        chatPanel.SetActive(false);
    }

    public void ShowDialogueBox(string name, string greeting)
    {
        chatPanel.SetActive(true);
        npcNameText.text = name;

        // Chạy hiệu ứng gõ chữ cho câu chào
        PlayTypingEffect(greeting);

        playerInputField.text = "";
        playerInputField.ActivateInputField();
    }

    public void HideDialogueBox()
    {
        chatPanel.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
    }

    // Hiển thị câu hỏi của người chơi ngay lập tức (Không cần gõ)
    public void DisplayUserMessage(string msg)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"<color=cyan>Bạn:</color> {msg}";
    }

    public void ShowLoadingIndicator()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"<color=gray><i>đang suy nghĩ...</i></color>";
    }

    // Hiển thị câu trả lời của NPC bằng hiệu ứng gõ
    public void DisplayAnswer(string answer)
    {
        PlayTypingEffect(answer);
    }

    private void PlayTypingEffect(string textToType)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypewriterEffect(textToType));
    }

    private IEnumerator TypewriterEffect(string text)
    {
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            // Dùng Realtime vì Time.timeScale đang bằng 0
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    public void OnSubmitButtonPressed()
    {
        string userText = playerInputField.text.Trim();
        if (string.IsNullOrEmpty(userText)) return;

        DisplayUserMessage(userText);
        DialogueManager.Instance.ProcessUserQuestion(userText);

        playerInputField.text = "";
        playerInputField.ActivateInputField();
    }

    private void CloseChat()
    {
        DialogueManager.Instance.EndDialogue();
    }
}