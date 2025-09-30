using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private float inputX;
    private bool isGrounded;
    private int facing = 1; // 1 = phải, -1 = trái

    // Animator params
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int FacingParam = Animator.StringToHash("Facing");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");

    // Damping cho Speed để blend mượt
    [SerializeField] private float speedDampTime = 0.08f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Input A/D (cũng nhận mũi tên trái/phải nếu cần)
        inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, 1

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Cập nhật hướng nhìn từ input mới nhất khác 0
        if (inputX != 0)
            facing = inputX > 0 ? 1 : -1;

        // Animator: Facing + (tuỳ chọn) IsGrounded
        anim.SetInteger(FacingParam, facing);
        anim.SetBool(IsGroundedParam, isGrounded);
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Move theo trục X
        float targetVelX = inputX * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelX, rb.linearVelocity.y);

        // Cập nhật Speed (dùng damping cho mượt)
        float speedAbs = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat(SpeedParam, speedAbs, speedDampTime, Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
