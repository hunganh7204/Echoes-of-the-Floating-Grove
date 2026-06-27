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
    public TextMeshProUGUI dialogueText; 
    public TMP_InputField playerInputField;
    public Button sendButton;
    public Button closeButton;

    [Header("Visual Novel Effect")]
    public Image npcAvatarImage;      
    public Image backgroundOverlay;   
    [SerializeField] private float avatarMoveOffset = 30f;
    [SerializeField] private Color normalOverlayColor = new Color(0, 0, 0, 0.4f); 
    [SerializeField] private Color focusOverlayColor = new Color(0, 0, 0, 0.75f); 

    private Vector2 originalAvatarPos;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.03f; 
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
            npcAvatarImage.preserveAspect = true;
        }

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

    public void DisplayUserMessage(string msg)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"Bạn: {msg}";
        SetFocusState(false);
    }

    public void ShowLoadingIndicator()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = $"Đang suy nghĩ...";
        //SetFocusState(true);
    }

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
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        SetFocusState(false);
    }

    private void SetFocusState(bool isFocused)
    {
        if (npcAvatarImage != null)
        {
            npcAvatarImage.rectTransform.anchoredPosition = isFocused
                ? originalAvatarPos + new Vector2(0, avatarMoveOffset)
                : originalAvatarPos;
        }

        if (backgroundOverlay != null)
        {
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