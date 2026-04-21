using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.2f); // Chiều ngang, chiều dọc của hộp kiểm tra
    [SerializeField] private LayerMask GroundLayer;

    [SerializeField]private Rigidbody2D rb;
    private bool isGravityInverted = false;
    private float lastUsedTime;

    public bool IsGravityInverted => isGravityInverted;

    private void Start()
    {
        lastUsedTime = -cooldownTime;
    }

    private void Update()
    {
        if (InputManager.Instance.GravityPressed)
        {
            TryInvertGravity();
        }
    }

    private void TryInvertGravity()
    {
        if (Time.time < lastUsedTime + cooldownTime)
        {
            return;
        }
        bool isCeilingBlocked = Physics2D.OverlapBox(ceilingCheck.position, boxSize, 0f, GroundLayer);

        if (isCeilingBlocked)
        {
            Debug.Log("Không đủ khoảng trống! Vai hoặc đầu sẽ bị kẹt!");
            return;
        }

        ExecuteGravityInversion();
    }

    private void ExecuteGravityInversion()
    {
        isGravityInverted = !isGravityInverted;
        lastUsedTime = Time.time;

        rb.gravityScale *= -1;
        transform.Rotate(180f, 0f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(ceilingCheck.position, boxSize);
        }
    }
}
