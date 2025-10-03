using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Sát thương mỗi đòn.")]
    [SerializeField] private int damage = 25;

    [Tooltip("Tốc độ tấn công (1.0 = bình thường, 1.5 = nhanh 50%).")]
    [SerializeField] private float attackSpeed = 1.0f;

    [Tooltip("Thời lượng chuẩn của animation tấn công (giây) ở tốc độ 1.0. Dùng để tính cooldown nếu không đặt event.")]
    [SerializeField] private float baseAttackDuration = 0.4f;

    [Tooltip("Thời điểm gây sát thương (0..1) theo tiến trình animation.")]
    [Range(0f, 1f)]
    [SerializeField] private float hitTimeNormalized = 0.35f;

    [Header("Hitbox")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private LayerMask enemyMask;

    [Header("Integration")]
    [Tooltip("Tên parameter hướng từ hệ thống di chuyển (-1 trái, 1 phải).")]
    [SerializeField] private string facingParam = "Facing";

    [Tooltip("Blend param: 0=move, 1=attack")]
    [SerializeField] private string actionParam = "Action";

    [Tooltip("Param điều tốc độ phát đòn (Speed Multiplier trên state).")]
    [SerializeField] private string attackSpeedParam = "AttackSpeed";

    [Tooltip("Khoảng lùi giữa các đòn (theo thời lượng đòn). 0 = spam tối đa theo attackSpeed.")]
    [SerializeField] private float extraRecovery = 0.0f;

    private Animator anim;
    private bool isAttacking;
    private int facing = 1; // đọc từ Animator để chọn trái/phải

    // Hash
    private int hashFacing, hashAction, hashAtkSpd;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hashFacing = Animator.StringToHash(facingParam);
        hashAction = Animator.StringToHash(actionParam);
        hashAtkSpd = Animator.StringToHash(attackSpeedParam);
    }

    void Update()
    {
        // Lấy hướng hiện tại từ Animator (đã được script Movement set)
        facing = anim.GetInteger(hashFacing);

        // Chuột trái để đánh
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Đẩy Blend Tree sang nhánh Attack và set tốc độ ra đòn
        anim.SetFloat(hashAtkSpd, Mathf.Max(0.05f, attackSpeed));
        anim.SetFloat(hashAction, 1f); // 1 = Attack node trong Blend Tree

        // Thời lượng thực tế theo attackSpeed
        float duration = Mathf.Max(0.05f, baseAttackDuration / Mathf.Max(0.05f, attackSpeed));
        float hitTime = Mathf.Clamp01(hitTimeNormalized) * duration;

        // Chờ tới khung impact
        yield return new WaitForSeconds(hitTime);

        // Gây sát thương
        DoHit();

        // Chờ phần còn lại + recovery
        float remain = duration - hitTime + extraRecovery;
        if (remain > 0f) yield return new WaitForSeconds(remain);

        // Trả Blend Tree về locomotion
        anim.SetFloat(hashAction, 0f);
        anim.SetFloat(hashAtkSpd, 1f);

        isAttacking = false;
    }

    private void DoHit()
    {
        if (!attackPoint) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);
        for (int i = 0; i < hits.Length; i++)
        {
            // Ví dụ: kẻ địch có component IDamageable
            var dmg = hits[i].GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }

    // Cho phép Animation Event gọi trực tiếp nếu bạn muốn sync 100% theo clip
    public void OnAttackHit() => DoHit();

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
#endif

    // API public để chỉnh runtime
    public void SetDamage(int val) => damage = Mathf.Max(0, val);
    public void SetAttackSpeed(float val) => attackSpeed = Mathf.Max(0.05f, val);
    public int Damage => damage;
    public float AttackSpeed => attackSpeed;
}
