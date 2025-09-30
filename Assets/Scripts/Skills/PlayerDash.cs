using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [Tooltip("Vận tốc lướt (units/second).")]
    [SerializeField] private float dashSpeed = 18f;

    [Tooltip("Khoảng cách lướt tối đa (units).")]
    [SerializeField] private float dashDistance = 4f;

    [Tooltip("Thời gian hồi chiêu (giây).")]
    [SerializeField] private float dashCooldown = 0.35f;

    [Tooltip("Phím kích hoạt lướt.")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    [Header("Collision Clamp (không bắt buộc)")]
    [Tooltip("Layer bị coi là chướng ngại khi lướt.")]
    [SerializeField] private LayerMask obstacleMask;

    [Tooltip("Khoảng trừ để không dính collider khi lướt (skin).")]
    [SerializeField] private float skin = 0.05f;

    [Header("Tích hợp")]
    [Tooltip("Nếu có, tạm tắt script di chuyển này khi lướt.")]
    [SerializeField] private MonoBehaviour movementToDisable;

    [Tooltip("Tên trigger Animator khi lướt (để trống nếu không dùng).")]
    [SerializeField] private string animatorDashTrigger = "Dash";

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDashing;
    private bool onCooldown;
    private int lastFacing = 1; // 1 = phải, -1 = trái
    private float defaultGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        // Cập nhật hướng từ input ngang gần nhất
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0) lastFacing = x > 0 ? 1 : -1;

        // Kích hoạt dash
        if (!isDashing && !onCooldown && Input.GetKeyDown(dashKey))
        {
            StartCoroutine(DashRoutine(lastFacing));
        }
    }

    private IEnumerator DashRoutine(int dirSign)
    {
        isDashing = true;
        onCooldown = true;

        // Tạm vô hiệu hoá điều khiển di chuyển khác (nếu có)
        if (movementToDisable) movementToDisable.enabled = false;

        // Raycast trước để không chui xuyên tường: giảm khoảng cách nếu có vật cản
        float allowedDistance = dashDistance;
        Vector2 origin = rb.position;
        Vector2 dir = new Vector2(dirSign, 0f);
        if (obstacleMask.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, dashDistance, obstacleMask);
            if (hit.collider != null)
            {
                allowedDistance = Mathf.Max(0f, hit.distance - skin);
            }
        }

        // Thời lượng lướt = quãng đường / tốc độ
        float duration = (dashSpeed > 0.001f) ? allowedDistance / dashSpeed : 0f;

        // Chuẩn bị trạng thái vật lý
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;                   // không bị rơi khi lướt
        rb.linearVelocity = Vector2.zero;             // reset để phản hồi tốt

        // Gọi animation (nếu có)
        if (anim && !string.IsNullOrEmpty(animatorDashTrigger))
            anim.SetTrigger(animatorDashTrigger);

        // Thực thi lướt: giữ vận tốc cho đến khi hết thời lượng hoặc đạt quãng đường
        float elapsed = 0f;
        Vector2 startPos = rb.position;
        while (elapsed < duration)
        {
            rb.linearVelocity = new Vector2(dirSign * dashSpeed, 0f);
            elapsed += Time.deltaTime;

            // Nếu vì lý do nào đó đã đi đủ quãng đường (va chạm cứng), thoát
            float traveled = Mathf.Abs(rb.position.x - startPos.x);
            if (traveled + 0.001f >= allowedDistance) break;

            yield return null;
        }

        // Kết thúc lướt
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.gravityScale = originalGravity;

        isDashing = false;

        // Bật lại điều khiển di chuyển
        if (movementToDisable) movementToDisable.enabled = true;

        // Hồi chiêu
        yield return new WaitForSeconds(dashCooldown);
        onCooldown = false;
    }

    // Cho script khác biết đang lướt để khoá input nếu muốn
    public bool IsDashing => isDashing;

    // Cho phép set hướng từ bên ngoài (ví dụ Animator/Movement)
    public void SetFacing(int facingSign) => lastFacing = Mathf.Sign(facingSign) >= 0 ? 1 : -1;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = new Color(0, 1, 1, 0.35f);
            Vector3 origin = transform.position;
            Gizmos.DrawLine(origin, origin + Vector3.right * dashDistance);
            Gizmos.DrawLine(origin, origin + Vector3.left * dashDistance);
        }
    }
#endif
}
