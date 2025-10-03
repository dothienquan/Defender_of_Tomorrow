// PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    int current;

    void Awake()
    {
        current = maxHealth;
    }

    public void TakeDamage(int amount, Vector2 hitPoint = default)
    {
        current -= amount;
        Debug.Log($"Player took {amount}, left {current}");
        // TODO: play hurt animation, knockback, UI update...
        if (current <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Player died");
        // player death logic
    }
}
