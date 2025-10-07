using UnityEngine;
using DG.Tweening;

public class PetFollower : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform player;

    [Header("Follow Settings")]
    [SerializeField] float smoothTime = 0.25f;     
    [SerializeField] float floatAmplitude = 0.3f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] Vector2 offset = new Vector2(1f, 1f);

    [Header("Random Motion")]
    [SerializeField] float randomRadius = 0.3f;

    [Header("Flip Settings")]
    [SerializeField] bool flipWithPlayer = true;
    [SerializeField] Transform flipRoot;

    private Vector3 velocity;
    private Vector3 baseOffset;
    private Vector3 lastPlayerPos;
    private float floatTimer;
    private bool isFacingLeft;

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("PetFollower2D: No player assigned!");
            enabled = false;
            return;
        }

        if (flipRoot == null)
            flipRoot = transform;

        baseOffset = offset;
        lastPlayerPos = player.position;
        StartFloating();
    }

    void Update()
    {
        if (player == null) return;

        // Flip with player movement
        if (flipWithPlayer)
        {
            float moveDir = player.position.x - lastPlayerPos.x;
            if (Mathf.Abs(moveDir) > 0.01f)
            {
                bool shouldFaceLeft = moveDir < 0;
                if (shouldFaceLeft != isFacingLeft)
                {
                    isFacingLeft = shouldFaceLeft;
                    FlipPet(isFacingLeft);
                }
            }
            lastPlayerPos = player.position;
        }

        // Floating and smooth follow
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 floatOffset = Vector3.up * Mathf.Sin(floatTimer) * floatAmplitude;

        Vector3 targetPos = player.position
                            + (Vector3)offset
                            + floatOffset
                            + (Vector3)Random.insideUnitCircle * randomRadius * 0.05f;

        // Smoothly move toward target
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    void FlipPet(bool faceLeft)
    {
        Vector3 scale = flipRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceLeft ? -1 : 1);
        flipRoot.localScale = scale;
    }

    void StartFloating()
    {
        floatTimer = Random.Range(0f, Mathf.PI * 2f);
    }
}
