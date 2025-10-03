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
    [SerializeField] private float dashCooldown = 1f;

    [Tooltip("Phím bấm để lướt (legacy Input).")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    [Header("Collision (optional)")]
    [Tooltip("Dừng sớm nếu gặp tường trong các Layer này. Bỏ trống để bỏ qua.")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private bool stopOnObstacle = true;

    // --- Runtime ---
    private Rigidbody2D rb;
    private bool isDashing;
    private bool onCooldown;
    private int lastFacing = 1; // -1 = trái, 1 = phải

    // --- Cooldown state for UI ---
    [SerializeField, Tooltip("Thời gian hồi còn lại (chỉ để xem trong Inspector).")]
    private float cooldownRemaining = 0f;

    /// <summary>Đang trong thời gian hồi chiêu?</summary>
    public bool OnCooldown => onCooldown;

    /// <summary>0..1, 1 = SẴN SÀNG, 0 = vừa bắt đầu hồi chiêu.</summary>
    public float CooldownNormalized
    {
        get
        {
            if (!onCooldown || dashCooldown <= 0f) return 1f;
            return 1f - Mathf.Clamp01(cooldownRemaining / dashCooldown);
        }
    }

    /// <summary>Giây còn lại trước khi dùng lại được.</summary>
    public float CooldownRemaining => Mathf.Max(0f, cooldownRemaining);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Xác định hướng nhìn gần nhất dựa theo input ngang (A/D, ←/→)
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

        // Chuẩn bị chiều và khoảng cách thực tế (có thể rút ngắn nếu gặp tường)
        Vector2 dir = new Vector2(Mathf.Sign(dirSign), 0f);
        float allowedDistance = dashDistance;

        if (stopOnObstacle && obstacleLayers.value != 0)
        {
            // Raycast để biết còn bao nhiêu khoảng trống phía trước
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, dashDistance, obstacleLayers);
            if (hit.collider != null)
            {
                // Trừ ra một khoảng rất nhỏ để không kẹt vào tường
                allowedDistance = Mathf.Max(0f, hit.distance - 0.02f);
            }
        }

        // Di chuyển bằng MovePosition theo FixedUpdate để ổn định
        float travelled = 0f;
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();
        while (travelled < allowedDistance)
        {
            float step = dashSpeed * Time.fixedDeltaTime;
            if (travelled + step > allowedDistance) step = allowedDistance - travelled;

            rb.MovePosition(rb.position + dir * step);
            travelled += step;

            // chờ tới khung vật lý tiếp theo
            yield return wffu;
        }

        isDashing = false;

        // --- HỒI CHIÊU có tiến trình để UI đọc ---
        cooldownRemaining = dashCooldown;
        while (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        cooldownRemaining = 0f;
        onCooldown = false;
    }

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
