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

    [Header("VFX")]
    [SerializeField] ParticleSystem trailVFX;        // normal looping VFX that follows pet
    [SerializeField] ParticleSystem startMoveVFX;    // burst when movement starts

    [Header("Flip Settings")]
    [SerializeField] bool flipWithPlayer = true;
    [SerializeField] Transform flipRoot;

    private Vector3 velocity;
    private Vector3 baseOffset;
    private Vector3 lastPlayerPos;
    private Vector3 lastTargetPos;
    private float floatTimer;
    private bool isFacingLeft;

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("PetFollower: No player assigned!");
            enabled = false;
            return;
        }

        if (flipRoot == null)
            flipRoot = transform;

        baseOffset = offset;
        lastPlayerPos = player.position;
        lastTargetPos = transform.position;
        StartFloating();

        // start looping trail effect
        if (trailVFX != null && !trailVFX.isPlaying)
            trailVFX.Play();
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

        // Floating effect
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 floatOffset = Vector3.up * Mathf.Sin(floatTimer) * floatAmplitude;

        // Calculate target position
        Vector3 targetPos = player.position
                            + (Vector3)offset
                            + floatOffset
                            + (Vector3)Random.insideUnitCircle * randomRadius * 0.05f;

        // Detect new movement -> play startMoveVFX
        if ((targetPos - lastTargetPos).sqrMagnitude > 0.05f)
        {
            if (startMoveVFX != null)
                startMoveVFX.Play();

            lastTargetPos = targetPos;
        }

        // Smooth follow
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
