using UnityEngine;

public class EnemyPatrolAttacker : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float waitAtPoint = 0.25f;

    [Header("Detection & Attack")]
    public float detectionRange = 4f;
    public float attackRange = 1.25f;     // a bit larger to be safe
    public int damage = 10;
    public float attackCooldown = 0.8f;
    [Tooltip("Optional: how close we try to stop when chasing")]
    public float stopDistance = 0.9f;

    [Header("Hit Settings")]
    public Transform attackPoint;         // place this at the hands/weapon
    public float attackRadius = 0.6f;

    [Header("Animation")]
    public Animator animator;             // Bool "IsMoving", Trigger "Attack"
    public bool flipSpriteByScale = true;

    [Header("Targeting")]
    public string playerTag = "Player";   // rely on tag, not layer

    // --- Internals / debug ---
    Transform _player;
    float _cooldownTimer;
    float _waitTimer;
    bool _goingToB = true;
    SpriteRenderer _sr;

    // Debug-only visibility in Inspector
    [SerializeField] float _debugDistanceToPlayer;
    [SerializeField] bool _debugInDetect, _debugInAttack;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!attackPoint) attackPoint = transform;

        FindPlayer();
        if (!pointA || !pointB)
            Debug.LogWarning($"{name}: Assign pointA/pointB for patrol.");
        if (!animator)
            Debug.LogWarning($"{name}: Animator reference is missing.");
    }

    void FindPlayer()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        _player = go ? go.transform : null;
        if (!_player)
            Debug.LogWarning($"{name}: Could not find a GameObject tagged '{playerTag}'.");
    }

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        if (!_player)
        {
            // Try refinding the player occasionally
            if (Time.frameCount % 60 == 0) FindPlayer();
            Patrol();
            return;
        }

        _debugDistanceToPlayer = Vector2.Distance(transform.position, _player.position);
        _debugInDetect = _debugDistanceToPlayer <= detectionRange;
        _debugInAttack = _debugDistanceToPlayer <= attackRange;

        if (_debugInDetect)
        {
            // Face & decide chase/attack
            Face(_player.position.x - transform.position.x);

            if (_debugInAttack)
            {
                SetMoving(false);
                TryAttack();  // logs when fired
            }
            else
            {
                // Chase but keep a small buffer (don’t overlap the player)
                Vector3 toPlayer = (_player.position - transform.position);
                float dist = toPlayer.magnitude;
                if (dist > stopDistance)
                {
                    Vector3 target = _player.position - toPlayer.normalized * stopDistance;
                    MoveTowards(target);
                }
                else
                {
                    SetMoving(false);
                }
            }
        }
        else
        {
            Patrol();
        }
    }

    void MoveTowards(Vector3 target)
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 delta = target - transform.position;
        if (delta.sqrMagnitude > 0.0001f)
        {
            Face(delta.x);
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            SetMoving(true);
        }
        else SetMoving(false);
    }

    void Patrol()
    {
        if (!pointA || !pointB) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            SetMoving(false);
            return;
        }

        Vector3 target = _goingToB ? pointB.position : pointA.position;
        float step = moveSpeed * Time.deltaTime;
        Vector3 delta = target - transform.position;

        if (delta.sqrMagnitude <= 0.001f)
        {
            _goingToB = !_goingToB;
            _waitTimer = waitAtPoint;
            SetMoving(false);
            return;
        }

        Face(delta.x);
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        SetMoving(true);
    }

    void Face(float xDir)
    {
        if (!flipSpriteByScale || _sr == null) return;
        if (Mathf.Abs(xDir) > 0.01f)
        {
            var s = _sr.transform.localScale;
            s.x = xDir > 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            _sr.transform.localScale = s;
        }
    }

    void SetMoving(bool moving)
    {
        if (animator) animator.SetBool("IsMoving", moving);
    }

    void TryAttack()
    {
        if (_cooldownTimer > 0f) return;
        _cooldownTimer = attackCooldown;

        if (animator)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
        Debug.Log($"{name}: Attack TRIGGER fired (dist={_debugDistanceToPlayer:F2})");

        // If you don’t use an Animation Event in the clip, keep immediate damage:
        DoDamage();
    }

    // Tag-based hit check (avoids layer mismatches on child colliders)
    public void DoDamage()
    {
        if (!attackPoint) attackPoint = transform;

        var hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        bool dealt = false;
        foreach (var h in hits)
        {
            if (!h) continue;
            var go = h.attachedRigidbody ? h.attachedRigidbody.gameObject : h.gameObject;

            // Accept either the collider or its root having the tag
            if (go.CompareTag(playerTag) || go.transform.root.CompareTag(playerTag))
            {
                var stats = go.GetComponent<CharacterStats>() ?? go.transform.root.GetComponent<CharacterStats>();
                if (stats)
                {
                    stats.TakeDamage(damage);
                    dealt = true;
                    Debug.Log($"{name}: Hit {go.name} for {damage} dmg via OverlapCircle.");
                    break;
                }
                else
                {
                    Debug.LogWarning($"{name}: Found '{go.name}' with tag '{playerTag}' but no CharacterStats.");
                }
            }
        }
        if (!dealt)
        {
            Debug.Log($"{name}: Attack swung but hit nothing. (attackPoint={attackPoint.position}, radius={attackRadius})");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Vector3 ap = attackPoint ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(ap, attackRadius);
        if (pointA && pointB)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
