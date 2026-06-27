using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    [Header("Cài đặt Cuộn ngang (Trục X)")]
    [SerializeField] private float parallaxMultiplierX = 0.5f;

    [Header("Cài đặt Cao độ (Trục Y)")]
    [SerializeField] private bool lockYToCamera = true;

    private Material mat;
    private Transform camTransform;
    private Vector3 startOffset;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;

        camTransform = Camera.main.transform;


        if (camTransform != null)
        {
            startOffset = transform.position - camTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (camTransform == null) return;

        float targetX = camTransform.position.x + startOffset.x;
        float targetY = lockYToCamera ? (camTransform.position.y + startOffset.y) : transform.position.y;

        transform.position = new Vector3(targetX, targetY, transform.position.z);


        float offsetX = camTransform.position.x * parallaxMultiplierX * 0.01f;
        mat.mainTextureOffset = new Vector2(offsetX, 0);
    }
}