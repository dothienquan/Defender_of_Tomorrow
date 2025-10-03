using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Runtime (được set khi spawn)")]
    public int damage = 20;
    public float speed = 12f;

    [Header("General")]
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private LayerMask hitMask;   // chọn Enemy/Obstacle tuỳ ý
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private GameObject hitVfx;   // tuỳ chọn

    private Vector2 moveDir = Vector2.right;
    private float lifeTimer;

    public void Init(Vector2 dir, int dmg, float spd)
    {
        moveDir = dir.normalized;
        damage = dmg;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;

        // Gây damage nếu đối tượng nhận được
        var dmg = other.GetComponent<IDamageable>();
        if (dmg != null) dmg.TakeDamage(damage);

        if (hitVfx) Instantiate(hitVfx, transform.position, Quaternion.identity);
        if (destroyOnHit) Destroy(gameObject);
    }
}
