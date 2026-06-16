using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 8f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField]private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isGrounded;
    private int jumpsLeft;

    [SerializeField] private Animator anim;

    private float currentHorizontalInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    anim = GetComponent<Animator>();
    //}

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsLeft = maxJumps;
        }

        currentHorizontalInput = InputManager.Instance.HorizontalMovement;
        if (InputManager.Instance.JumpPressed)
        {
            if (isGrounded || jumpsLeft > 0)
            {
                Jump();
            }
        }


        Flip();
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        if (anim == null) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 1. Khóa khi đang thực hiện các hành động ưu tiên cao
        if (stateInfo.IsName("ATTACK") || stateInfo.IsName("DAMAGED") || stateInfo.IsName("DEATH") || anim.IsInTransition(0))
        {
            return;
        }

        // 2. Bộ lọc nhiễu: Chỉ coi là di chuyển nếu bấm nút và vận tốc trục X thực sự lớn
        bool isMovingHorizontally = Mathf.Abs(currentHorizontalInput) > 0.1f && Mathf.Abs(rb.linearVelocity.x) > 0.5f;

        if (isGrounded && isMovingHorizontally)
        {
            if (!stateInfo.IsName("MOVE"))
            {
                anim.Play("MOVE");
            }
        }
        else if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.5f) // Tăng độ bao dung cho trục Y lên 0.5f để lọc nhiễu sàn gạch
        {
            if (!stateInfo.IsName("IDLE") && Mathf.Abs(rb.linearVelocity.x) < 0.5f)
            {
                anim.Play("IDLE");
            }
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float currentSpeed = baseSpeed;
        if (InputManager.Instance.SprintHeld)
        {
            currentSpeed *= sprintMultiplier;
        }

        rb.linearVelocity = new Vector2(currentHorizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        if(isFacingRight && currentHorizontalInput < 0f || !isFacingRight && currentHorizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        jumpsLeft--;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
