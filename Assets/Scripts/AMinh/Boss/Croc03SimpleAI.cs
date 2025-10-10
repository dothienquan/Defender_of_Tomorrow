using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Croc03ChaseRB : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float flipDeadzone = 0.35f;
    [SerializeField] int forwardSign = +1;

    Rigidbody2D rb;
    float baseScaleX; bool facingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScaleX = Mathf.Abs(transform.localScale.x);
        facingRight = transform.localScale.x * forwardSign >= 0f;
    }

    void FixedUpdate()
    {
        if (!target) return;

        float dx = target.position.x - transform.position.x;

        // Flip mặt có deadzone
        if (dx > flipDeadzone) facingRight = true;
        else if (dx < -flipDeadzone) facingRight = false;

        var s = transform.localScale;
        s.x = baseScaleX * (facingRight ? +1f : -1f) * forwardSign;
        transform.localScale = s;

        // Hướng chạy
        float dir = Mathf.Abs(dx) > 0.05f ? Mathf.Sign(dx) : 0f;

        // Vận tốc theo X (giữ Y theo gravity)
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }
}
