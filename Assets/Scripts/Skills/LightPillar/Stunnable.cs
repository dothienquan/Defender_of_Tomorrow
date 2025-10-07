using System.Collections;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
    [Tooltip("Scripts to disable while stunned (optional). If empty the system will try to disable EnemyPatrolAttacker if present.")]
    public MonoBehaviour[] behavioursToDisable;

    Coroutine _stunRoutine;

    // External helper used by LightPillar fallback
    public void ForceStunExternal(EnemyPatrolAttacker epa, float duration)
    {
        // if behavioursToDisable empty, add epa
        if (behavioursToDisable == null || behavioursToDisable.Length == 0)
        {
            behavioursToDisable = new MonoBehaviour[] { epa };
        }
        Stun(duration);
    }

    public void Stun(float duration)
    {
        if (_stunRoutine != null) StopCoroutine(_stunRoutine);
        _stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        // disable listed behaviours
        foreach (var b in behavioursToDisable)
        {
            if (b != null) b.enabled = false;
        }

        // Optionally disable animator or set a flag — adjust as needed
        var animator = GetComponentInChildren<Animator>();
        if (animator) animator.SetFloat("Stunned", 1f); // if you use animator param

        yield return new WaitForSeconds(duration);

        // re-enable
        foreach (var b in behavioursToDisable)
        {
            if (b != null) b.enabled = true;
        }

        if (animator) animator.SetFloat("Stunned", 0f);
        _stunRoutine = null;
    }
}
