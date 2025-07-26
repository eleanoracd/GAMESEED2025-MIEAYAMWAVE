using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform platformCheck;
    [SerializeField] private float platformCheckRadius = 0.2f;

    private Rigidbody2D rb;
    public bool IsGrounded { get; private set; }
    public bool IsJumping => !IsGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckPlatform();

        if (InputHandler.Instance.JumpPressed && IsGrounded)
        {
            Jump();
        }

        if (InputHandler.Instance.PausePressed)
        {
            Debug.Log("Pause Pressed!");
        }

        InputHandler.Instance.ResetFlags();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !InputHandler.Instance.JumpHeld)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    internal void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void CheckPlatform()
    {
        IsGrounded = Physics2D.OverlapCircle(platformCheck.position, platformCheckRadius, platformLayer);
    }
}
