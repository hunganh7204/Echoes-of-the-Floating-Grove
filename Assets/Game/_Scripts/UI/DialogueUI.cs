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

    [Header("Visual Novel Effect")]
    public Image npcAvatarImage;      // Kéo UI Image hiển thị NPC vào đây
    public Image backgroundOverlay;   // Kéo UI Image làm nền tối vào đây
    [SerializeField] private float avatarMoveOffset = 30f; // Khoảng cách đẩy ảnh lên
    [SerializeField] private Color normalOverlayColor = new Color(0, 0, 0, 0.4f); // Tối vừa
    [SerializeField] private Color focusOverlayColor = new Color(0, 0, 0, 0.75f); // Tối đậm

    private Vector2 originalAvatarPos;

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
        if (npcAvatarImage != null)
            originalAvatarPos = npcAvatarImage.rectTransform.anchoredPosition;

        chatPanel.SetActive(false);
        if (backgroundOverlay != null) backgroundOverlay.gameObject.SetActive(false);
        if (npcAvatarImage != null) npcAvatarImage.gameObject.SetActive(false);
    }

    public void ShowDialogueBox(string name, string greeting, Sprite npcSprite)
    {
        chatPanel.SetActive(true);
        npcNameText.text = name;

        if (backgroundOverlay != null) backgroundOverlay.gameObject.SetActive(true);
        if (npcAvatarImage != null && npcSprite != null)
        {
            npcAvatarImage.sprite = npcSprite;
            npcAvatarImage.gameObject.SetActive(true);
            // Giữ tỷ lệ gốc của ảnh NPC để không bị méo
            npcAvatarImage.preserveAspect = true;
        }

        // Chạy hiệu ứng gõ chữ cho câu chào
        PlayTypingEffect(greeting);

        playerInputField.text = "";
        playerInputField.ActivateInputField();
    }

    public void HideDialogueBox()
    {
        chatPanel.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (backgroundOverlay != null) backgroundOverlay.gameObject.SetActive(false);
        if (npcAvatarImage != null) npcAvatarImage.gameObject.SetActive(false);

        SetFocusState(false);
    }

    // Hiển thị câu hỏi của người chơi ngay lập tức (Không cần gõ)
    public void DisplayUserMessage(string msg)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"<color=cyan>Bạn:</color> {msg}";
        SetFocusState(false);
    }

    public void ShowLoadingIndicator()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"<color=gray><i>đang suy nghĩ...</i></color>";
        //SetFocusState(true);
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
        SetFocusState(true);
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            // Dùng Realtime vì Time.timeScale đang bằng 0
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        SetFocusState(false);
    }

    private void SetFocusState(bool isFocused)
    {
        if (npcAvatarImage != null)
        {
            // Nhô lên nếu isFocused, ngược lại về vị trí gốc
            npcAvatarImage.rectTransform.anchoredPosition = isFocused
                ? originalAvatarPos + new Vector2(0, avatarMoveOffset)
                : originalAvatarPos;
        }

        if (backgroundOverlay != null)
        {
            // Đổi màu nền (Tối đậm / Tối vừa)
            backgroundOverlay.color = isFocused ? focusOverlayColor : normalOverlayColor;
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