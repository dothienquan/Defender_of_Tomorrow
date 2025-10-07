using System.Collections;
using UnityEngine;

public class LightPillarAbility : MonoBehaviour
{
    [Header("Binding")]
    public KeyCode castKey = KeyCode.Q;

    [Header("Pillar")]
    public GameObject pillarPrefab;
    public Transform spawnOrigin;
    public float forwardSpacing = 1.5f;
    public int pillarCount = 3;
    public float timeBetweenPillars = 0.18f;
    public float pillarLifetime = 1.0f;

    [Header("Stun")]
    public float stunDuration = 3.0f;

    // ---- NEW: đọc hướng từ Animator của player (PlayerMovement đang set param "Facing") ----
    [Header("Facing via Animator")]
    public Animator playerAnimator;               // kéo thả Animator của Player (hoặc để trống để auto-find)
    private static readonly int FacingParam = Animator.StringToHash("Facing");

    void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInParent<Animator>(); // hoặc GetComponent<Animator>() nếu script đặt trên Player
    }

    void Update()
    {
        if (Input.GetKeyDown(castKey))
            StartCoroutine(CastPillars());
    }

    IEnumerator CastPillars()
    {
        if (pillarPrefab == null || spawnOrigin == null) yield break;

        Vector3 forward = GetFacingForward();

        for (int i = 0; i < pillarCount; i++)
        {
            Vector3 spawnPos = spawnOrigin.position + forward * forwardSpacing * (i + 1);
            GameObject g = Instantiate(pillarPrefab, spawnPos, Quaternion.identity);

            var lp = g.GetComponent<LightPillar>();
            if (lp != null) lp.stunDuration = stunDuration;

            // Nếu muốn cột “quay mặt” theo hướng bắn (cho sprite/VFX)
            if (Mathf.Abs(forward.x) > 0.001f)
            {
                var sc = g.transform.localScale;
                sc.x = Mathf.Sign(forward.x) * Mathf.Abs(sc.x);
                g.transform.localScale = sc;
            }

            Destroy(g, pillarLifetime + 0.1f);
            yield return new WaitForSeconds(timeBetweenPillars);
        }
    }

    Vector3 GetFacingForward()
    {
        if (playerAnimator != null)
        {
            int facing = playerAnimator.GetInteger(FacingParam); // 1 = phải, -1 = trái (do PlayerMovement set)
            if (facing != 0) return facing > 0 ? Vector3.right : Vector3.left;
        }

        // Fallback nếu thiếu Animator/param: dùng hướng transform
        return spawnOrigin.right.x >= 0 ? Vector3.right : Vector3.left;
    }
}
