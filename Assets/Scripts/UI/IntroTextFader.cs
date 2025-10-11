// IntroTextFader.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if TMP_PRESENT

#endif

[DisallowMultipleComponent]
public class IntroTextFader : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
#if TMP_PRESENT
    [SerializeField] private TMP_Text tmpText;    // prefer TMP
#endif
    [SerializeField] private TextMeshProUGUI uiText;         // fallback if not using TMP

    [Header("Behavior")]
    [TextArea] public string introText = "Wind Domain";
    public float fadeInTime = 0.6f;
    public float holdTime = 1.4f;
    public float fadeOutTime = 1.0f;
    public bool deactivateOnEnd = true;
    public bool allowSkip = true;         // any key/click to skip

    bool _running;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Awake()
    {
#if TMP_PRESENT
        if (tmpText) tmpText.text = introText;
#endif
        if (uiText) uiText.text = introText;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
    }

    void Start()
    {
        if (!_running) StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        _running = true;

        // ensure text set
#if TMP_PRESENT
        if (tmpText) tmpText.text = introText;
#endif
        if (uiText) uiText.text = introText;

        // Fade in
        yield return FadeTo(1f, fadeInTime);

        // Hold (skippable)
        float t = 0f;
        while (t < holdTime)
        {
            if (allowSkip && Input.anyKeyDown) break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out (skippable)
        if (allowSkip && Input.anyKeyDown) { /* skip */ }
        else yield return FadeTo(0f, fadeOutTime);

        if (deactivateOnEnd) gameObject.SetActive(false);
        _running = false;
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(t / duration));
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    // Optional: call to replay (e.g., when reloading)
    public void Play(string overrideText = null)
    {
        if (!string.IsNullOrEmpty(overrideText)) introText = overrideText;
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
        StartCoroutine(PlayRoutine());
    }
}
