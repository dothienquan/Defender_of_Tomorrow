using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class FlyingPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float moveDuration = 3f;
    public Ease easeType = Ease.InOutSine;
    [Tooltip("Rotate 180° around Y each time it reaches a point.")]
    public bool flipYOnTurn = true;

    private Tween moveTween;
    private bool flipped = false;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning($"{name}: Missing patrol points!");
            return;
        }

        // Start at point A
        transform.position = pointA.position;

        // Start patrol loop
        moveTween = transform.DOMove(pointB.position, moveDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(OnReachedPoint);
    }

    void OnReachedPoint()
    {
        if (flipYOnTurn)
        {
            flipped = !flipped;
            float yRot = flipped ? 180f : 0f;
            transform.DORotate(new Vector3(0, yRot, 0), 0.3f, RotateMode.Fast);
        }
    }

    void OnDestroy()
    {
        moveTween?.Kill();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
#endif
}
