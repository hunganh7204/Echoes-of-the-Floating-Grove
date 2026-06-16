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
        if (!isSaved)
        {
            Debug.Log("UI: Nhấn [F] để Lưu trò chơi (Nghỉ ngơi)");
        }
    }

    public void HideInteractionPrompt()
    {
        Debug.Log("UI: Ẩn nhắc nhở Lưu trò chơi");

        // Trả lại màu cũ (hoặc giữ nguyên màu cam nếu bạn muốn lửa cứ cháy mãi sau khi save)
        if (!isSaved)
        {
            spriteRenderer.color = Color.white;
        }
    }
}