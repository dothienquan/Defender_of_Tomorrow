// SpawnOnActive.cs
// When this GameObject is enabled (SetActive(true)), spawn a prefab and animate it with DOTween.
// Supports 2D world (SpriteRenderer fade) and UI (CanvasGroup fade).

using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnOnActive : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject prefab;         // What to spawn
    [SerializeField] private Transform spawnParent;     // Optional parent for the spawned object
    [SerializeField] private Transform spawnPoint;      // Optional spawn position/rotation; falls back to this.transform

    [Header("Timing")]
    [SerializeField] private bool spawnEveryEnable = true; // If false, spawns only once (first enable)
    [SerializeField] private float spawnDelay = 0f;        // Delay before spawning

    [Header("Animation")]
    [SerializeField] private bool useScalePop = true;
    [SerializeField] private Vector3 startScale = new Vector3(0f, 0f, 1f);
    [SerializeField] private Vector3 endScale = Vector3.one;

    [SerializeField] private bool useFade = true;      // Tries CanvasGroup first, else SpriteRenderer
    [SerializeField] private float startAlpha = 0f;
    [SerializeField] private float endAlpha = 1f;

    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool ignoreTimeScale = true;

    [Header("Cleanup")]
    [SerializeField] private bool killTweensOnDisable = true;

    private bool hasSpawnedOnce = false;
    private GameObject lastInstance;
    private Tween scaleT, fadeT;

    void OnEnable()
    {
        if (!spawnEveryEnable && hasSpawnedOnce) return;
        hasSpawnedOnce = true;
        if (prefab == null)
        {
            Debug.LogWarning($"[SpawnOnActive] No prefab assigned on {name}");
            return;
        }
        if (spawnDelay > 0f) Invoke(nameof(SpawnNow), spawnDelay);
        else SpawnNow();
    }

    void OnDisable()
    {
        if (killTweensOnDisable)
        {
            scaleT?.Kill();
            fadeT?.Kill();
            scaleT = fadeT = null;
        }
    }

    private void SpawnNow()
    {
        Transform origin = spawnPoint != null ? spawnPoint : transform;

        lastInstance = Instantiate(prefab, origin.position, origin.rotation, spawnParent != null ? spawnParent : null);

        // Try to prepare for animation
        var instRT = lastInstance.GetComponent<RectTransform>();
        var instCG = lastInstance.GetComponent<CanvasGroup>();
        var instSR = lastInstance.GetComponent<SpriteRenderer>();

        // SCALE POP
        if (useScalePop)
        {
            // If UI, scale on RectTransform; else on Transform
            if (instRT != null)
                instRT.localScale = startScale;
            else
                lastInstance.transform.localScale = startScale;

            var target = (instRT != null) ? (Tween)instRT.DOScale(endScale, duration)
                                          : lastInstance.transform.DOScale(endScale, duration);
            scaleT = target.SetEase(ease).SetUpdate(ignoreTimeScale);
        }

        // FADE (CanvasGroup preferred; else SpriteRenderer)
        if (useFade)
        {
            if (instCG != null)
            {
                instCG.alpha = startAlpha;
                fadeT = instCG.DOFade(endAlpha, duration).SetEase(ease).SetUpdate(ignoreTimeScale);
            }
            else if (instSR != null)
            {
                var c = instSR.color;
                instSR.color = new Color(c.r, c.g, c.b, startAlpha);
                fadeT = instSR.DOFade(endAlpha, duration).SetEase(ease).SetUpdate(ignoreTimeScale);
            }
            // If neither exists, you can add a CanvasGroup/SpriteRenderer to your prefab to enable fading.
        }
    }

    // Optional: call this from other scripts if you want manual re-spawn
    public GameObject SpawnImmediate()
    {
        SpawnNow();
        return lastInstance;
    }

    // Optional: access the last spawned instance
    public GameObject LastInstance => lastInstance;

    // Optional: cancel delayed spawn if you disable/destroy early
    void OnDestroy()
    {
        if (spawnDelay > 0f) CancelInvoke(nameof(SpawnNow));
    }
}
