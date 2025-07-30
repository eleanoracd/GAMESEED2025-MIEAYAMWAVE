using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Cultist : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float jumpDelay = 0.01f;
    private bool jumpQueued = false;
    private float timeSinceJumpCommand = 0f;
    private const float maxJumpDelay = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float deceleration = 01f;
    [SerializeField] private float positionTolerance = 0.1f;
    private float targetXPosition;


    [Header("Cover Settings")]
    [SerializeField] private float coverDuration = 1f;

    public enum CultistType { Leader, Follower }
    private CultistType cultistType = CultistType.Follower;


    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        InputHandler.OnJump += HandleJump;
        InputHandler.OnCover += HandleCover;
    }

    private void OnDisable()
    {
        InputHandler.OnJump -= HandleJump;
        InputHandler.OnCover -= HandleCover;

        jumpQueued = false;
    }

    private void HandleJump()
    {
        if (!IsGrounded()) return;

        if (IsLeader())
        {
            Jump();
            StartCoroutine(MakeFollowersJump());
        }
    }

    private void HandleHorizontalMovement()
    {
        if (!IsGrounded()) return;

        float distanceToTarget = targetXPosition - transform.position.x;
        float currentDirection = Mathf.Sign(rb.velocity.x);
        float targetDirection = Mathf.Sign(distanceToTarget);

        bool shouldAccelerate = (Mathf.Abs(distanceToTarget) > positionTolerance) && 
                            (Mathf.Abs(rb.velocity.x) < maxSpeed || 
                            Mathf.Sign(rb.velocity.x) != targetDirection);

        float accelerationRate = shouldAccelerate ? acceleration : deceleration;
        float targetSpeed = shouldAccelerate ? maxSpeed * targetDirection : 0f;
        
        float newX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, accelerationRate * Time.deltaTime);
        rb.velocity = new Vector2(newX, rb.velocity.y);

        if (Mathf.Abs(distanceToTarget) <= positionTolerance)
        {
            transform.position = new Vector3(targetXPosition, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void Update()
    {
        if (jumpQueued && !IsLeader())
        {
            timeSinceJumpCommand += Time.deltaTime;

            if (IsGrounded())
            {
                Jump();
                jumpQueued = false;
            }
            else if (timeSinceJumpCommand > maxJumpDelay)
            {
                jumpQueued = false;
            }
        }

        HandleHorizontalMovement();
    }

    private void HandleCover()
    {
        StartCoroutine(CoverRoutine());
    }

    private IEnumerator CoverRoutine()
    {
        col.enabled = false;
        yield return new WaitForSeconds(coverDuration);
        col.enabled = true;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private IEnumerator MakeFollowersJump()
    {
        CultistManager cultistManager = FindObjectOfType<CultistManager>();
        if (cultistManager == null) yield break;

        List<GameObject> activeCultists = cultistManager.GetActiveCultists();

        yield return new WaitForSeconds(jumpDelay);

        for (int i = 0; i < activeCultists.Count; i++)
        {
            GameObject cultistObj = activeCultists[i];
            Cultist cultist = cultistObj.GetComponent<Cultist>();
            if (cultist != null && !cultist.IsLeader())
            {
                cultist.QueueJump();
                
                if (i < activeCultists.Count - 1)
                {
                    yield return new WaitForSeconds(jumpDelay);
                }
            }
        }
    }

    public void QueueJump()
    {
        jumpQueued = true;
        timeSinceJumpCommand = 0f;
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void SetAsLeader()
    {
        cultistType = CultistType.Leader;
    }

    public void SetAsFollower()
    {
        cultistType = CultistType.Follower;
    }

    public bool IsLeader()
    {
        return cultistType == CultistType.Leader;
    }
    
    public void SetTargetXPosition(float xPosition)
    {
        targetXPosition = xPosition;
    }
    
}