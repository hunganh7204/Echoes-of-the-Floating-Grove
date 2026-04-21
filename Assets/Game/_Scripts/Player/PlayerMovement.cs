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

    private float currentHorizontalInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
