using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Vector2 boxSize = new Vector2(0.8f, 0.2f);
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private int manaCost = 15;

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
            return;
        }

        ExecuteGravityInversion();
    }

    private void ExecuteGravityInversion()
    {
        if (!PlayerStats.Instance.UseMana(manaCost)) return;
        isGravityInverted = !isGravityInverted;
        lastUsedTime = Time.time;

        rb.gravityScale *= -1;
        transform.Rotate(180f, 0f, 0f);
    }
    public void ResetGravity()
    {
        if (isGravityInverted)
        {
            isGravityInverted = false;
            rb.gravityScale = Mathf.Abs(rb.gravityScale); 
            transform.rotation = Quaternion.identity;     
            Debug.Log("Player: Đã khôi phục phương hướng trọng lực.");
        }
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
