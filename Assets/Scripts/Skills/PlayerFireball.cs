using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerFireball : MonoBehaviour
{
    [Header("Cast Settings")]
    [SerializeField] private int fireballDamage = 30;
    [SerializeField] private float fireballSpeed = 14f;

    [Tooltip("Thời gian cast chuẩn (giây). Dùng để tính delay sinh đạn nếu không đặt Animation Event.")]
    [SerializeField] private float baseCastDuration = 0.45f;

    [Tooltip("Thời điểm sinh đạn theo tỉ lệ 0..1 của hoạt ảnh.")]
    [Range(0f, 1f)]
    [SerializeField] private float spawnTimeNormalized = 0.35f;

    [Tooltip("Hồi chiêu giữa 2 lần bắn (giây).")]
    [SerializeField] private float castCooldown = 0.2f;

    [Header("Spawn")]
    [SerializeField] private Transform firePoint;                     // đặt trước tay nhân vật
    [SerializeField] private FireballProjectile fireballPrefab;       // kéo prefab vào

    [Header("Integration")]
    [SerializeField] private string facingParam = "Facing";           // -1 / 1
    [SerializeField] private string actionParam = "Action";           // 0 move, 1 melee, 2 cast
    [SerializeField] private KeyCode castKey = KeyCode.Mouse1;        // Chuột phải

    private Animator anim;
    private bool isCasting;
    private bool onCooldown;

    private int hashFacing, hashAction;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hashFacing = Animator.StringToHash(facingParam);
        hashAction = Animator.StringToHash(actionParam);
    }

    void Update()
    {
        if (!isCasting && !onCooldown && Input.GetKeyDown(castKey))
        {
            StartCoroutine(CastRoutine());
        }
    }

    private IEnumerator CastRoutine()
    {
        isCasting = true;
        onCooldown = true;

        // Đặt Blend Tree sang Cast
        anim.SetFloat(hashAction, 2f);

        // Tính thời lượng thực (nếu bạn có Speed Multiplier riêng có thể áp dụng ở đây)
        float duration = Mathf.Max(0.05f, baseCastDuration);
        float spawnDelay = Mathf.Clamp01(spawnTimeNormalized) * duration;

        // Chờ tới khung spawn
        yield return new WaitForSeconds(spawnDelay);

        SpawnFireball();

        // Chờ hết animation còn lại
        float remain = duration - spawnDelay;
        if (remain > 0f) yield return new WaitForSeconds(remain);

        // Trả về locomotion
        anim.SetFloat(hashAction, 0f);

        // Hồi chiêu
        yield return new WaitForSeconds(castCooldown);
        onCooldown = false;
        isCasting = false;
    }

    private void SpawnFireball()
    {
        if (!fireballPrefab || !firePoint) return;

        // Lấy hướng từ Animator
        int facing = anim.GetInteger(hashFacing);  // -1 hoặc 1
        Vector2 dir = new Vector2(Mathf.Sign(facing == 0 ? 1 : facing), 0f);

        var proj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        proj.Init(dir, fireballDamage, fireballSpeed);

        // Xoay sprite đạn theo hướng (nếu muốn)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Cho Animation Event nếu bạn thích đồng bộ trực tiếp từ clip
    public void OnFireballSpawnEvent() => SpawnFireball();

    // Expose cho chỉnh runtime
    public void SetFireballDamage(int dmg) => fireballDamage = Mathf.Max(0, dmg);
    public void SetFireballSpeed(float spd) => fireballSpeed = Mathf.Max(0.01f, spd);

    public int FireballDamage => fireballDamage;
    public float FireballSpeed => fireballSpeed;
}
