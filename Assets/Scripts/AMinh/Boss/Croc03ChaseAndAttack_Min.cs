using UnityEngine;

public class Croc03ChaseAndAttack_Min : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform target;
    [SerializeField] Animator anim;

    [Header("Move")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float flipDeadzone = 0.35f; // tránh lật lia
    [SerializeField] int forwardSign = +1;     // +1 nếu sprite gốc nhìn PHẢI, -1 nếu nhìn TRÁI

    [Header("Attack")]
    [SerializeField] float stopRange = 2.5f;   // đứng lại
    [SerializeField] float attackRange = 2.0f;   // trong tầm đánh
    [SerializeField] float atkCD = 2.0f;
    [SerializeField] float altCD = 3.0f;

    float baseScaleX; bool facingRight;
    float nextAtk, nextAlt;

    void Reset()
    {
        if (!anim) anim = GetComponent<Animator>();
    }

    void Awake()
    {
        baseScaleX = Mathf.Abs(transform.localScale.x);
        facingRight = transform.localScale.x * forwardSign >= 0f;
    }

    void Update()
    {
        if (!target || !anim) return;

        float dx = target.position.x - transform.position.x;
        float adx = Mathf.Abs(dx);

        // 1) Flip mặt có deadzone (hết rung mặt)
        if (dx > flipDeadzone) facingRight = true;
        else if (dx < -flipDeadzone) facingRight = false;
        var s = transform.localScale;
        s.x = baseScaleX * (facingRight ? +1f : -1f) * forwardSign;
        transform.localScale = s;

        // 2) Nếu đang Attack/AttackAlt → để animation tự chạy, cuối clip nó sẽ về Idle
        var st = anim.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("Attack") || st.IsName("AttackAlt"))
        {
            // Không di chuyển khi đang ra đòn
            return;
        }

        // 3) Quyết định di chuyển/đánh
        if (adx > stopRange)
        {
            // Di chuyển theo WORLD (không dính flip)
            float dir = Mathf.Sign(dx);
            transform.Translate(new Vector3(dir * moveSpeed * Time.deltaTime, 0f, 0f), Space.World);
            // Có thể để Idle/Walk tuỳ bạn, nhưng với controller tối giản ta cứ để Idle
            return;
        }

        // Trong tầm đánh
        if (adx <= attackRange)
        {
            if (Time.time >= nextAlt)
            {
                anim.ResetTrigger("Attack");
                anim.SetTrigger("Fire");     // sẽ phát AttackAlt
                nextAlt = Time.time + altCD;
                return;
            }
            if (Time.time >= nextAtk)
            {
                anim.ResetTrigger("Fire");
                anim.SetTrigger("Attack");   // sẽ phát Attack
                nextAtk = Time.time + atkCD;
                return;
            }
        }

        // Nếu đứng sát mà chưa tới CD → đứng yên (Idle)
    }
}
