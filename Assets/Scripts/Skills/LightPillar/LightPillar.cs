using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class LightPillar : MonoBehaviour
{
    [Tooltip("LayerMask của đối tượng được ảnh hưởng (ví dụ: Enemy)")]
    public LayerMask affectedLayers;
    public float stunDuration = 3f;
    public bool oneHitPerEntity = true;

    // optional VFX/audio hooks
    public ParticleSystem onSpawnVfx;
    public ParticleSystem onHitVfx;

    // to avoid double-hitting same target multiple times if desired
    private System.Collections.Generic.HashSet<GameObject> _hitSet = new System.Collections.Generic.HashSet<GameObject>();

    void Start()
    {
        if (onSpawnVfx) onSpawnVfx.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Layer filter
        if (((1 << other.gameObject.layer) & affectedLayers.value) == 0) return;

        GameObject target = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (oneHitPerEntity && _hitSet.Contains(target)) return;
        _hitSet.Add(target);

        // Try to find Stunnable component first
        var st = target.GetComponent<Stunnable>() ?? target.transform.root.GetComponent<Stunnable>();
        if (st != null)
        {
            st.Stun(stunDuration);
            if (onHitVfx) onHitVfx.Play();
            return;
        }

        // Fallback: try to disable EnemyPatrolAttacker directly (if present)
        var epa = target.GetComponent<EnemyPatrolAttacker>() ?? target.transform.root.GetComponent<EnemyPatrolAttacker>();
        if (epa != null)
        {
            // add a temporary Stunnable if desired so we can restore properly
            var tmp = target.AddComponent<Stunnable>();
            tmp.ForceStunExternal(epa, stunDuration);
            if (onHitVfx) onHitVfx.Play();
            return;
        }

        // Other interactions (e.g., player) can be added here.
    }
}
