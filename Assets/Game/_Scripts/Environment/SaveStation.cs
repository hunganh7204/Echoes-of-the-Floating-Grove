using UnityEngine;

public class SaveStation : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    private bool isSaved = false; // Thêm cờ để tránh lưu liên tục nhiều lần nếu không cần thiết

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void Interact()
    {
        // Đổi màu lửa sáng lên
        spriteRenderer.color = new Color(1f, 0.5f, 0f);
        Debug.Log("<color=orange>Hiệu ứng: Đống lửa lưu game bùng cháy!</color>");

        // 1. Lấy vị trí trạm làm điểm Checkpoint
        Vector2 checkpointPos = transform.position;

        // 2. Xin ID màn chơi hiện tại từ Tổng đài Map
        int currentLevel = LevelGenerator.Instance.CurrentLevel;

        // 3. Gọi DataManager lưu toàn bộ xuống JSON
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