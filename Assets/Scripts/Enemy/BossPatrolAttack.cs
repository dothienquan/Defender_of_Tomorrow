using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossPatrolAttack : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float waitAtPoint = 0.25f;

    [Header("Detection & Attack")]
    public float detectionRange = 4f;
    public float attackRange = 3f;     // enemy sẽ bắn trong tầm này
    public float attackCooldown = 1.2f;
    public float stopDistance = 1f;


    [Header("Wind Skill")]
    public GameObject windPrefab;
    public Transform firePoint;
    public float windDelay = 0.1f;     // trễ một chút sau animation Attack

    [Header("Animation")]
    public Animator animator;             // Bool "IsMoving", Trigger "Attack"
    public bool flipSpriteByScale = true;

    [Header("Targeting")]
    public string playerTag = "Player";

    // --- Internal fields ---
    Transform _player;
    float _cooldownTimer;
    float _waitTimer;
    bool _goingToB = true;
    SpriteRenderer _sr;

    [SerializeField] float _debugDistanceToPlayer;
    [SerializeField] bool _debugInDetect, _debugInAttack;

    void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!firePoint) firePoint = transform;

        FindPlayer();

        if (!pointA || !pointB)
            Debug.LogWarning($"{name}: Assign pointA/pointB for patrol.");
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
            if (Time.frameCount % 60 == 0) FindPlayer();
            Patrol();
            return;
        }

        _debugDistanceToPlayer = Vector2.Distance(transform.position, _player.position);
        _debugInDetect = _debugDistanceToPlayer <= detectionRange;
        _debugInAttack = _debugDistanceToPlayer <= attackRange;

        if (_debugInDetect)
        {
            Face(_player.position.x - transform.position.x);

            if (_debugInAttack)
            {
                SetMoving(false);
                TryAttack();
            }
            else
            {
                Vector3 toPlayer = (_player.position - transform.position);
                float dist = toPlayer.magnitude;
                if (dist > stopDistance)
                {
                    Vector3 target = _player.position - toPlayer.normalized * stopDistance;
                    MoveTowards(target);
                }
                else SetMoving(false);
            }
        }
        else
        {
            Patrol();
        }
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

        // Bắn gió sau 1 chút để khớp animation
       
    }

    void ShootWind()
    {
        if (windPrefab && firePoint)
        {
            GameObject wind = Instantiate(windPrefab, firePoint.position, firePoint.rotation);

            // Nếu enemy đang quay trái thì xoay gió ngược lại
            if (_sr != null && _sr.transform.localScale.x < 0)
            {
                var s = wind.transform.localScale;
                s.x *= -1;
                wind.transform.localScale = s;

                // Xoay hướng đạn
                wind.transform.right = -firePoint.right;
            }
        }
        else
        {
            Debug.LogWarning($"{name}: windPrefab hoặc firePoint chưa gán!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
