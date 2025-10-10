using UnityEngine;

public class Croc03AnimEvents : MonoBehaviour
{
    public Croc03AnimDriver anim;
    public Croc03Combat combat; // chỗ bạn xử lý damage/hitbox

    public void AE_AttackHit() { combat?.SpawnAttackHitbox(); }
    public void AE_AtkEnd()
    {
        anim.PlayWalk(1f);                 // quay lại đi bộ
    }
    /*public void AE_DeathEnd() { combat?.OnDeathFinished(); }*/
}
