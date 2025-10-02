using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Thời gian miễn nhiễm (giây).")]
    [SerializeField] private float shieldDuration = 3f;   // bạn yêu cầu 3s

    [Tooltip("Thời gian hồi chiêu (giây).")]
    [SerializeField] private float shieldCooldown = 6f;

    [Tooltip("Phím kích hoạt lá chắn.")]
    [SerializeField] private KeyCode shieldKey = KeyCode.Q;

    [Header("Collision (tuỳ chọn)")]
    [Tooltip("Bật để tạm thời bỏ qua va chạm với layer của đòn tấn công/kẻ địch.")]
    [SerializeField] private bool ignoreCollisionsWhileShield = true;

    [Tooltip("Layer của Player (ví dụ: Player).")]
    [SerializeField] private int playerLayer = 8;

    [Tooltip("Layer tấn công/kẻ địch (ví dụ: Enemy, EnemyAttack).")]
    [SerializeField] private int[] hostileLayers = new int[] { 9 };   // thay bằng layer của bạn

    [Header("VFX / Animator (tuỳ chọn)")]
    [Tooltip("GameObject VFX của khiên (bật/tắt khi dùng).")]
    [SerializeField] private GameObject shieldVfx;

    [Tooltip("Tên Trigger animation khi bật khiên.")]
    [SerializeField] private string shieldAnimTrigger = "Shield";

    private Animator anim;
    private bool isActive;
    private bool onCooldown;

    public bool IsActive => isActive;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (shieldVfx) shieldVfx.SetActive(false);
    }

    void Update()
    {
        if (!isActive && !onCooldown && Input.GetKeyDown(shieldKey))
        {
            Debug.Log("Shield Activated");
            StartCoroutine(ShieldRoutine());
        }
    }

    private IEnumerator ShieldRoutine()
    {
        isActive = true;
        onCooldown = true;

        // Animation / VFX
        if (anim && !string.IsNullOrEmpty(shieldAnimTrigger))
            anim.SetTrigger(shieldAnimTrigger);
        if (shieldVfx) shieldVfx.SetActive(true);

        // Bỏ qua va chạm nếu chọn chế độ B
        if (ignoreCollisionsWhileShield)
        {
            foreach (var hostile in hostileLayers)
                Physics2D.IgnoreLayerCollision(playerLayer, hostile, true);
        }

        // Chờ hết thời gian lá chắn
        yield return new WaitForSeconds(Mathf.Max(0.01f, shieldDuration));

        // Tắt hiệu ứng
        if (shieldVfx) shieldVfx.SetActive(false);
        if (ignoreCollisionsWhileShield)
        {
            foreach (var hostile in hostileLayers)
                Physics2D.IgnoreLayerCollision(playerLayer, hostile, false);
        }

        isActive = false;

        // Hồi chiêu
        yield return new WaitForSeconds(Mathf.Max(0f, shieldCooldown));
        onCooldown = false;
    }
}
