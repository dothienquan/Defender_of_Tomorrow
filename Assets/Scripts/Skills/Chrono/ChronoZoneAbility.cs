using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoZoneAbility : MonoBehaviour
{
    [Header("Input")]
    public KeyCode castKey = KeyCode.V;

    [Header("Spawn")]
    public GameObject zonePrefab;     // Prefab có CircleCollider2D (isTrigger=true) + ChronoZone.cs
    public Transform spawnOrigin;     // thường là transform player
    public float castDistance = 3f;   // khoảng cách trước mặt để đặt vùng

    [Header("Facing via Animator (tùy chọn)")]
    public Animator playerAnimator;   // nếu PlayerMovement set param "Facing" = -1/1
    private static readonly int FacingParam = Animator.StringToHash("Facing");

    [Header("Facing via Sprite (fallback)")]
    public SpriteRenderer playerSprite;  // nếu bạn dùng flipX

    private void Awake()
    {
        if (spawnOrigin == null) spawnOrigin = transform;
        if (playerAnimator == null) playerAnimator = GetComponentInParent<Animator>();
        if (playerSprite == null) playerSprite = GetComponentInParent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(castKey))
        {
            Vector3 fwd = GetFacingForward();
            Vector3 pos = spawnOrigin.position + fwd * castDistance;
            Instantiate(zonePrefab, pos, Quaternion.identity);
        }
    }

    Vector3 GetFacingForward()
    {
        // Ưu tiên Animator param "Facing" nếu có
        if (playerAnimator != null)
        {
            int facing = playerAnimator.GetInteger(FacingParam);
            if (facing != 0) return facing > 0 ? Vector3.right : Vector3.left;
        }
        // Fallback: Sprite flipX
        if (playerSprite != null) return playerSprite.flipX ? Vector3.left : Vector3.right;
        // Cuối: dùng right của spawnOrigin
        return spawnOrigin.right.x >= 0 ? Vector3.right : Vector3.left;
    }
}
