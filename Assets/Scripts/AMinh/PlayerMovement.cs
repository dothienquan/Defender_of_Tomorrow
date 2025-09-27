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

    [Tooltip("Chặn nhảy trong vài ms đầu để tránh bật lên khi Play.")]
    public float startupGraceTime = 0.15f;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;

    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;
    private bool isGrounded;
    private float startTime;

    // Tên state trong Animator (đổi nếu bạn dùng tên khác)
    private const string IdleStateName = "Idle";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    void Update()
    {
        // --- Input ---
        moveInput = Input.GetAxisRaw("Horizontal"); // Old Input System
        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;

        // --- Ground check ---
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
            if (isGrounded) lastGroundedTime = Time.time;
        }
        else
        {
            // Fallback nếu quên gán groundCheck
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        // --- Gravity feel / variable jump ---
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime);
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (1f - lowJumpMultiplier) * Time.deltaTime);
        }

        // --- Try Jump (buffer + coyote) ---
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool bufferedJump = (Time.time - lastJumpPressedTime) <= jumpBuffer;
        bool passedStartup = (Time.time - startTime) >= startupGraceTime;

        if (passedStartup && bufferedJump && (isGrounded || canCoyote))
            DoJump();

        // --- Animator ---
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Di chuyển ngang
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void DoJump()
    {
        lastJumpPressedTime = -999f;
        lastGroundedTime = -999f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void UpdateAnimation()
    {
        // 1) Cập nhật grounded
        animator.SetBool("IsGrounded", isGrounded);

        // 2) Tính cờ chạy theo input (chỉ khi đang đứng đất)
        bool left = isGrounded && moveInput < -0.01f;
        bool right = isGrounded && moveInput > 0.01f;

        // 3) Set trực tiếp 2 cờ hướng (không reset rồi set lại)
        animator.SetBool("IsRunningLeft", left);
        animator.SetBool("IsRunningRight", right);

        // 4) Nếu đang đứng đất và KHÔNG chạy trái/phải -> ép về Idle
        if (isGrounded && !left && !right)
        {
            const string IdleState = "Idle"; // ĐỔI nếu state Idle của bạn tên khác!
            var s = animator.GetCurrentAnimatorStateInfo(0);
            if (!s.IsName(IdleState))
                animator.CrossFade(IdleState, 0.05f, 0);
        }
        // Trên không: chỉ cần IsGrounded=false, Animator tự sang Jump theo transition
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
