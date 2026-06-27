using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FixedResponse
{
    public string keyword; 
    [TextArea(2, 4)]
    public string answer; 
}

public class NPCAI : MonoBehaviour, IInteractable
{
    [Header("1. Thông tin cơ bản")]
    public string npcName = "Testing";

    [TextArea(2, 4)]
    public string greetingMessage = "Ngươi tìm ta có việc gì, Kẻ lang thang?"; 

    [Header("2. Thiết lập AI")]
    [TextArea(3, 5)]
    public string npcPersona = "Bạn là một trưởng lão thông thái trong hầm ngục. Bạn luôn trả lời ngắn gọn, bí ẩn và xưng 'Ta'.";

    [TextArea(3, 5)]
    public string npcLore = "Nơi này là Hầm ngục lãng quên. Chúa tể Vực thẳm đã bị phong ấn ở tầng 3. Chìa khóa giấu trong phòng dung nham.";

    [Header("3. Kịch bản cứng")]
    public List<FixedResponse> fixedResponses;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider.isTrigger = true;
    }

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(this);
        HideInteractionPrompt();
    }

    public void ShowInteractionPrompt()
    {
        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.ShowPrompt("Trò chuyện", transform);
    }

    public void HideInteractionPrompt()
    {
        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.HidePrompt();
    }

    public Sprite GetSprite()
    {
        if (spriteRenderer != null) return spriteRenderer.sprite;
        return null;
    }
}