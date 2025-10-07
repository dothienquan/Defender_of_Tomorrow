using UnityEngine;

public class WindSlash : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifeTime = 2f;

    [Header("Attack Settings")]
    public int damage = 10;
    public float knockbackForce = 7f;
    public float knockbackUp = 3f;
    public GameObject hitEffect;

    private Rigidbody2D rb;
    private bool hasHit = false;
    private int moveDir = 1; // 1 = phải, -1 = trái

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Xác định hướng bay dựa trên scale
        moveDir = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDir * speed, 0f);

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            // --- Gây sát thương ---
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // --- Đẩy lùi nhân vật ---
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = new Vector2(moveDir, 0.4f).normalized;
                Vector2 finalForce = (knockDir * knockbackForce) + (Vector2.up * knockbackUp);
                playerRb.AddForce(finalForce, ForceMode2D.Impulse);
            }

            // --- Hiệu ứng va chạm ---
            if (hitEffect)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject, 0.05f);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
