using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    float hp;

    void Awake() { hp = maxHP; }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        Debug.Log($"Player HP: {hp}");
        if (hp <= 0) Debug.Log("Player died.");
    }
}
