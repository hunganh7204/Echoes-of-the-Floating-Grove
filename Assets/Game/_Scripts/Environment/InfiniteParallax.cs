using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    [Header("Cài đặt Cuộn ngang (Trục X)")]
    [Tooltip("Hệ số cuộn Texture. Bầu trời = 0.1 (Chậm), Rừng = 0.5 (Nhanh)")]
    [SerializeField] private float parallaxMultiplierX = 0.5f;

    [Header("Cài đặt Cao độ (Trục Y)")]
    [Tooltip("Khóa chặt vào Camera để ảnh không bao giờ bị lọt xuống dưới đáy.")]
    [SerializeField] private bool lockYToCamera = true;

    private Material mat;
    private Transform camTransform;
    private Vector3 startOffset;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;

        // Tự động tìm Camera chính
        camTransform = Camera.main.transform;

        // Lưu lại khoảng cách gốc giữa bức ảnh và Camera ngay từ đầu
        if (camTransform != null)
        {
            startOffset = transform.position - camTransform.position;
        }
    }

    // BẮT BUỘC dùng LateUpdate để đồng bộ hoàn hảo với Camera
    private void LateUpdate()
    {
        if (camTransform == null) return;

        // 1. GIỮ BỨC ẢNH LUÔN NẰM TRONG KHUNG HÌNH (Không bị bỏ lại phía sau)
        float targetX = camTransform.position.x + startOffset.x;
        float targetY = lockYToCamera ? (camTransform.position.y + startOffset.y) : transform.position.y;

        transform.position = new Vector3(targetX, targetY, transform.position.z);

        // 2. TẠO HIỆU ỨNG CHIỀU SÂU 3D BẰNG CÁCH TRƯỢT TEXTURE
        // (Đây là lý do bạn cần set Mesh Type = Full Rect và Wrap Mode = Repeat)
        float offsetX = camTransform.position.x * parallaxMultiplierX * 0.01f;
        mat.mainTextureOffset = new Vector2(offsetX, 0);
    }
}