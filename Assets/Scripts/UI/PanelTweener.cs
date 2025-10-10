// PanelTweener.cs
// Smooth show/hide UI panels with DOTween (fade, slide, pop).
// Add this to your panel GameObject (needs RectTransform). A CanvasGroup will be added if missing.

using System;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class PanelTweener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup cg;          // for fade + raycast toggle
    [SerializeField] private RectTransform rt;        // for slide/scale

    [Header("Behavior")]
    [SerializeField] private bool startHidden = true; // start inactive/hidden
    [SerializeField] private bool useFade = true;
    [SerializeField] private bool useScalePop = false;
    [SerializeField] private bool useSlide = false;

    [Header("Timings")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private bool ignoreTimeScale = true;

    [Header("Slide Settings (if useSlide)")]
    [SerializeField] private Vector2 hiddenAnchoredPos = new Vector2(0, -800); // off-screen pos
    private Vector2 shownAnchoredPos;

    [Header("Scale Settings (if useScalePop)")]
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.9f, 0.9f, 1f);
    private readonly Vector3 shownScale = Vector3.one;

    public bool IsShown { get; private set; }

    Tween fadeT, moveT, scaleT;

    void Reset()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
    }

    void Awake()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (cg == null) cg = GetComponent<CanvasGroup>();
        shownAnchoredPos = rt.anchoredPosition;

        if (startHidden) ApplyHiddenState(immediate: true);
        else ApplyShownState(immediate: true);
    }

    // -------- Public API --------
    public void Toggle()
    {
        if (IsShown) Hide();
        else Show();
    }

    public void Show()
    {
        KillTweens();

        // Ensure active before animating
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // Start values
        if (useFade) cg.alpha = Mathf.Clamp01(cg.alpha);     // keep current if already 0..1
        if (useSlide) rt.anchoredPosition = hiddenAnchoredPos;
        if (useScalePop) rt.localScale = hiddenScale;

        // Tweens
        if (useFade) fadeT = cg.DOFade(1f, duration);
        if (useSlide) moveT = rt.DOAnchorPos(shownAnchoredPos, duration);
        if (useScalePop) scaleT = rt.DOScale(shownScale, duration);

        ConfigureTweens(null, fadeT, moveT, scaleT);
        SetRaycast(true);
        IsShown = true;
    }

    public void Hide()
    {
        KillTweens();

        // Tweens to hidden state
        if (useFade) fadeT = cg.DOFade(0f, duration);
        if (useSlide) moveT = rt.DOAnchorPos(hiddenAnchoredPos, duration);
        if (useScalePop) scaleT = rt.DOScale(hiddenScale, duration);

        ConfigureTweens(() =>
        {
            // Finally disable to fully hide from hierarchy & save draw calls
            gameObject.SetActive(false);
        }, fadeT, moveT, scaleT);

        SetRaycast(false);
        IsShown = false;
    }

    // -------- Internals --------
    private void ApplyHiddenState(bool immediate)
    {
        if (useFade) cg.alpha = 0f;
        if (useSlide) rt.anchoredPosition = hiddenAnchoredPos;
        if (useScalePop) rt.localScale = hiddenScale;

        SetRaycast(false);
        if (immediate && gameObject.activeSelf) gameObject.SetActive(false);
        IsShown = false;
    }

    private void ApplyShownState(bool immediate)
    {
        if (useFade) cg.alpha = 1f;
        if (useSlide) rt.anchoredPosition = shownAnchoredPos;
        if (useScalePop) rt.localScale = shownScale;

        SetRaycast(true);
        if (immediate && !gameObject.activeSelf) gameObject.SetActive(true);
        IsShown = true;
    }

    private void SetRaycast(bool on)
    {
        if (cg == null) return;
        cg.blocksRaycasts = on;
        cg.interactable = on;
    }

    private void ConfigureTweens(Action onComplete, params Tween[] tweens)
    {
        Tween master = null;
        foreach (var t in tweens)
        {
            if (t == null) continue;
            t.SetEase(ease)
             .SetUpdate(ignoreTimeScale);
            master = t; // last non-null is fine; durations are equal
        }
        if (master != null && onComplete != null)
            master.OnComplete(() => onComplete());
    }

    private void KillTweens()
    {
        fadeT?.Kill();
        moveT?.Kill();
        scaleT?.Kill();
        fadeT = moveT = scaleT = null;
    }
}
