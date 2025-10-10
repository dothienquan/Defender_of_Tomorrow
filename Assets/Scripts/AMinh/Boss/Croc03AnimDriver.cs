using UnityEngine;

[DisallowMultipleComponent]
public class Croc03AnimDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Hash tránh sai chính tả + nhanh hơn
    static readonly int H_Attack = Animator.StringToHash("Attack");
    static readonly int H_Fire = Animator.StringToHash("Fire");
    static readonly int H_Action = Animator.StringToHash("Action");
    static readonly int H_State = Animator.StringToHash("State");
    static readonly int H_Speed = Animator.StringToHash("Speed");

    // Quy ước State int
    public enum St { Ready = 0, Walk = 1, Attack = 2, AttackAlt = 3, Death = 4 }

    void Reset() { if (!animator) animator = GetComponentInChildren<Animator>(); }

    // ====== TRIGGERS ======
    public void TriggerAttack()
    {
        animator.SetBool(H_Action, true);
        animator.SetInteger(H_State, (int)St.Attack);
        animator.SetTrigger(H_Attack);
    }

    public void TriggerAttackAlt()
    {
        animator.SetBool(H_Action, true);
        animator.SetInteger(H_State, (int)St.AttackAlt);
        animator.SetTrigger(H_Fire);
    }

    public void TriggerDeath()
    {
        animator.ResetTrigger(H_Attack);
        animator.ResetTrigger(H_Fire);
        animator.SetBool(H_Action, true);
        animator.SetInteger(H_State, (int)St.Death);
        // Nếu có transition AnyState→Death theo State/Trigger là sẽ phát ngay
    }

    // ====== STATE/LOCOMOTION ======
    public void PlayReady()
    {
        animator.SetBool(H_Action, false);
        animator.SetInteger(H_State, (int)St.Ready);
    }

    public void PlayWalk(float moveSpeed01)
    {
        animator.SetBool(H_Action, false);
        animator.SetInteger(H_State, (int)St.Walk);
        animator.SetFloat(H_Speed, Mathf.Clamp01(moveSpeed01));
    }

    // ====== Helpers (nếu cần) ======
    public bool IsInAttack()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName("Attack") || info.IsName("AttackAlt");
    }

    public bool AnimAlmostDone(float t = 0.9f)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime >= t;
    }
}
