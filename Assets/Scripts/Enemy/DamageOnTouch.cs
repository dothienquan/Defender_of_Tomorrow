using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int touchDamage = 5;
    public LayerMask playerLayer;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & playerLayer) != 0)
        {
            var stats = col.gameObject.GetComponent<CharacterStats>();
            if (stats) stats.TakeDamage(touchDamage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            var stats = other.GetComponent<CharacterStats>();
            if (stats) stats.TakeDamage(touchDamage);
        }
    }
}
