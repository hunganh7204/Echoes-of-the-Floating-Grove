using UnityEngine;

public class SaveStation : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    private bool isSaved = false; 

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void Interact()
    {
        
        spriteRenderer.color = new Color(1f, 0.5f, 0f);     
        Vector2 checkpointPos = transform.position;      
        int currentLevel = LevelGenerator.Instance.CurrentLevel;
        DataManager.Instance.SaveGame(currentLevel, checkpointPos);

        isSaved = true;
    }

    public void ShowInteractionPrompt()
    {
        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.ShowPrompt("Lưu tiến trình", transform);
    }

    public void HideInteractionPrompt()
    {
        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.HidePrompt();
    }
}