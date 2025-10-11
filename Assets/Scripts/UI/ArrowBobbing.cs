// ArrowBobbing.cs
// Make any object bob up/down (or any axis) using DOTween.

using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class ArrowBobbing : MonoBehaviour
{
    public enum SpaceMode { Local, World }
    public SpaceMode space = SpaceMode.Local;

    [Header("Motion")]
    public Vector3 axis = Vector3.up;   // up/down by default
    public float amplitude = 0.25f;     // how far it moves
    public float duration = 0.8f;       // time for one up or down
    public Ease ease = Ease.InOutSine;
    public float startDelay = 0f;
    public bool ignoreTimeScale = true; // continue when paused

    [Header("Optional: subtle scale breathing")]
    public bool scalePulse = false;
    public float scaleAmount = 0.05f;   // 5% pulse
    public float scaleDuration = 0.8f;

    private Vector3 startPos;
    private Tween moveT, scaleT;

    void OnEnable()
    {
        startPos = space == SpaceMode.Local ? transform.localPosition : transform.position;

        var toPos = startPos + axis.normalized * amplitude;

        if (space == SpaceMode.Local)
            moveT = transform.DOLocalMove(toPos, duration)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(ease)
                            .SetDelay(startDelay)
                            .SetUpdate(ignoreTimeScale);
        else
            moveT = transform.DOMove(toPos, duration)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(ease)
                            .SetDelay(startDelay)
                            .SetUpdate(ignoreTimeScale);

        if (scalePulse)
        {
            var baseScale = transform.localScale;
            scaleT = transform.DOScale(baseScale * (1f + scaleAmount), scaleDuration)
                              .SetLoops(-1, LoopType.Yoyo)
                              .SetEase(Ease.InOutSine)
                              .SetUpdate(ignoreTimeScale);
        }
    }

    void OnDisable()
    {
        moveT?.Kill();
        scaleT?.Kill();
        // Reset to exact start to avoid drift
        if (space == SpaceMode.Local) transform.localPosition = startPos;
        else transform.position = startPos;
    }
}
