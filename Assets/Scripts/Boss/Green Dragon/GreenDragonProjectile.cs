using UnityEngine;

public class GreenDragonProjectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private Vector2 dir;

    public void Launch(Vector2 target, float spd, float dmg)
    {
        speed = spd;
        damage = dmg;
        dir = (target - (Vector2)transform.position).normalized;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        else if (!col.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
