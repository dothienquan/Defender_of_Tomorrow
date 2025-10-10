using UnityEngine;

public class Croc03Combat : MonoBehaviour
{
    
    public LayerMask targetMask;                 // Layer của Player
    public Transform origin;                     // Điểm tạo hitbox (nếu null dùng transform)
    public Vector2 boxSize = new(1.6f, 1.0f);    // Kích thước hitbox
    public Vector2 boxOffset = new(1.0f, 0.5f);  // Lệch về phía trước mặt
    public int damage = 1;

    // ==== Animation Event gọi đúng HÀM NÀY ====
    public void SpawnAttackHitbox()
    {
        var o = origin ? origin : transform;
        float dir = Mathf.Sign(transform.lossyScale.x);
        Vector2 center = (Vector2)o.position + new Vector2(boxOffset.x * dir, boxOffset.y);

        foreach (var col in Physics2D.OverlapBoxAll(center, boxSize, 0f, targetMask))
        {
            // Không ép kiểu gì thêm — gọi theo tên hàm nếu có
            col.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
