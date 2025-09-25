using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float waitTimeAtPoint = 0.5f;

    [Header("Detection & Attack")]
    public Transform attackPoint;
    public float aggroRange = 4f;
    public float attackRange = 0.8f;
    public int attackDamage = 10;
    public float attackCooldown = 1.2f;
    public LayerMask playerLayer;

    [Header("Misc")]
    public bool spriteFacingRight = true; // gốc sprite nhìn phải? (nếu gốc nhìn trái thì untick)

    Animator anim;
    Rigidbody2D rb;

    Vector3 targetPos;
    Transform player;
    bool isDead = false;
    bool facingRight = true;
    float waitTimer = 0f;
    float attackTimer = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("Patrol points not assigned on " + name);
        }
        targetPos = pointB ? pointB.position : transform.position;

        // Gán hướng mặc định theo sprite gốc
        facingRight = spriteFacingRight;
        ApplyFacing();
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        // Look for player (aggro)
        Collider2D found = Physics2D.OverlapCircle(transform.position, aggroRange, playerLayer);
        if (found)
        {
            player = found.transform;
        }
        else
        {
            player = null;
        }

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);

                if (attackTimer <= 0f)
                {
                    anim.SetTrigger("Attack");
                    attackTimer = attackCooldown;
                }
            }
            else
            {
                MoveTowards(player.position);
            }
        }
        else
        {
            Patrol();
        }

        // 👉 luôn xoay mặt theo hướng di chuyển
        if (rb.linearVelocity.x > 0.05f && !facingRight) Flip();
        if (rb.linearVelocity.x < -0.05f && facingRight) Flip();
    }

    void MoveTowards(Vector3 pos)
    {
        Vector2 dir = (pos - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void Patrol()
    {
        if (pointA == null || pointB == null) return;

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            if (waitTimer <= 0f) waitTimer = waitTimeAtPoint;
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                targetPos = (targetPos == pointA.position) ? pointB.position : pointA.position;
            }
            return;
        }

        MoveTowards(targetPos);
    }

    void Flip()
    {
        facingRight = !facingRight;
        ApplyFacing();
    }

    void ApplyFacing()
    {
        // Nếu sprite gốc quay phải thì facingRight=true = Y=0
        // Nếu sprite gốc quay trái thì facingRight=true = Y=180
        float yRotation;
        if (spriteFacingRight)
            yRotation = facingRight ? 0f : 180f;
        else
            yRotation = facingRight ? 180f : 0f;

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (var hit in hits)
        {
            IDamageable dam = hit.GetComponent<IDamageable>();
            if (dam != null)
            {
                dam.TakeDamage(attackDamage, transform.position);
            }
            else
            {
                var ph = hit.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(attackDamage);
            }
        }
    }

    public void OnHitReaction()
    {
        anim.SetTrigger("Hit");
    }

    public void SetDead()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDead", true);
        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
