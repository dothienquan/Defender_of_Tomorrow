using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChronoZone : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeTime = 3f;

    [Header("Effects")]
    [Range(0.1f, 1f)] public float slowMultiplier = 0.5f; // 0.5 = giảm 50% tốc khi ở trong vùng
    public float stunDuration = 3f;

    [Header("Filters")]
    public LayerMask enemyLayers;     // set layer Enemy
    public string playerTag = "Player";

    private readonly HashSet<GameObject> enemiesInside = new HashSet<GameObject>();
    private bool hasTriggeredStun = false;

    private void Start()
    {
        // đảm bảo collider là trigger
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        Destroy(gameObject, lifeTime);
    }

    private void OnDestroy()
    {
        // gỡ slow khỏi tất cả khi vùng biến mất
        foreach (var e in enemiesInside)
        {
            if (e == null) continue;
            var slow = e.GetComponent<SlowEffect>();
            if (slow) slow.RemoveSource(this);
        }
        enemiesInside.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var go = GetRoot(other);
        if (go == null) return;

        if (IsEnemy(go))
        {
            enemiesInside.Add(go);

            var slow = go.GetComponent<SlowEffect>();
            if (slow == null) slow = go.AddComponent<SlowEffect>();
            slow.AddOrUpdateSource(this, slowMultiplier);
        }
        else if (IsPlayer(go))
        {
            // Player vào vùng: stun tất cả enemy đang ở trong
            if (!hasTriggeredStun)
            {
                hasTriggeredStun = true;
                StunAllEnemiesInside();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var go = GetRoot(other);
        if (go == null) return;

        if (IsEnemy(go))
        {
            enemiesInside.Remove(go);
            var slow = go.GetComponent<SlowEffect>();
            if (slow) slow.RemoveSource(this);
        }
    }

    void StunAllEnemiesInside()
    {
        foreach (var e in enemiesInside)
        {
            if (e == null) continue;

            // ưu tiên Stunnable (đã cung cấp trước đó)
            var st = e.GetComponent<Stunnable>();
            if (st != null)
            {
                st.Stun(stunDuration);
                continue;
            }

            // fallback: nếu có EnemyPatrolAttacker thì thêm tạm Stunnable rồi stun
            var epa = e.GetComponent<EnemyPatrolAttacker>() ?? e.GetComponentInChildren<EnemyPatrolAttacker>();
            if (epa != null)
            {
                var tmp = e.AddComponent<Stunnable>();
                tmp.ForceStunExternal(epa, stunDuration);
            }
        }
    }

    bool IsEnemy(GameObject go)
    {
        return (enemyLayers.value & (1 << go.layer)) != 0;
    }

    bool IsPlayer(GameObject go)
    {
        return go.CompareTag(playerTag);
    }

    GameObject GetRoot(Collider2D col)
    {
        if (col == null) return null;
        return col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;
    }
}
