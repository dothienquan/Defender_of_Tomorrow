using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 12f;
    [Min(0f)] public float coyoteTime = 0.1f;
    [Min(0f)] public float jumpBuffer = 0.1f;
    public float fallGravityMultiplier = 2f;
    [Range(0.1f, 1f)] public float lowJumpMultiplier = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayers = ~0;

    [Tooltip("Avoid auto-jumping when entering Play.")]
    public float startupGraceTime = 0.15f;

    [Header("Visuals")]
    public bool flipSpriteOnMove = true; // if you use one Run clip + SpriteRenderer flip

    Rigidbody2D rb;
    Animator animator;

    float moveInput;
    float lastGroundedTime = -999f;
    float lastJumpPressedTime = -999f;
    bool isGrounded;
    float startTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    void Update()
    {
        // --- Input ---
        moveInput = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 using Old Input
        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;

        // --- Ground check ---
        if (groundCheck)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
            if (isGrounded) lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        // --- Extra gravity / variable jump feel ---
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime);
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (1f - lowJumpMultiplier) * Time.deltaTime);
        }

        // --- Buffered + Coyote Jump ---
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool bufferedJump = (Time.time - lastJumpPressedTime) <= jumpBuffer;
        bool passedStartup = (Time.time - startTime) >= startupGraceTime;

        if (passedStartup && bufferedJump && (isGrounded || canCoyote))
            DoJump();

        // --- Animator parameters for Blend Tree / Air ---
        // MoveX = -1..1 (sign of intended direction). Keep 0 when no input.
        float moveXParam = Mathf.Approximately(moveInput, 0f) ? 0f : Mathf.Sign(moveInput);

        // Speed = 0..1 (normalized ground horizontal speed). Keep 0 while airborne so Ground tree sits at Idle.
        float speed01 = isGrounded
            ? Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.x) / Mathf.Max(0.001f, moveSpeed))
            : 0f;

        animator.SetFloat("MoveX", moveXParam);
        animator.SetFloat("Speed", speed01);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelY", rb.linearVelocity.y);

        // Optional: flip sprite if you only have a right-facing run
        if (flipSpriteOnMove)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr && !Mathf.Approximately(moveInput, 0f))
                sr.flipX = moveInput < 0f;
        }
    }

    void FixedUpdate()
    {
        // Horizontal move
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void DoJump()
    {
        lastJumpPressedTime = -999f;
        lastGroundedTime = -999f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
