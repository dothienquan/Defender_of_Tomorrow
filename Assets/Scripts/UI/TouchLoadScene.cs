// TouchLoadNext.cs
// Put this on a 2D trigger (e.g., a portal/door). Shows a panel, waits 2–3s, then loads next scene.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TouchLoadNext : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool oneUse = true;

    [Header("UI")]
    [Tooltip("Panel to enable while 'loading' (fake). Optional.")]
    [SerializeField] private GameObject loadingPanel;

    [Header("Timing")]
    [SerializeField] private float minDelaySeconds = 2f;
    [SerializeField] private float maxDelaySeconds = 3f;
    [Tooltip("Use realtime so delay still works if Time.timeScale = 0")]
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Scene Switch")]
    [Tooltip("How many scenes ahead to load (1 = next).")]
    [SerializeField] private int nextSceneOffset = 1;
    [Tooltip("If true and index goes past last scene, wrap to 0.")]
    [SerializeField] private bool wrapBuildIndex = false;

    private bool _busy;
    private bool _used;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_busy || (_used && oneUse)) return;
        if (!other.CompareTag(playerTag)) return;

        StartCoroutine(LoadNextRoutine());
    }

    private IEnumerator LoadNextRoutine()
    {
        _busy = true;
        _used = true;

        if (loadingPanel) loadingPanel.SetActive(true);

        float wait = Random.Range(minDelaySeconds, maxDelaySeconds);
        if (useUnscaledTime) yield return WaitRealtime(wait);
        else yield return new WaitForSeconds(wait);

        int current = SceneManager.GetActiveScene().buildIndex;
        int target = current + nextSceneOffset;
        int count = SceneManager.sceneCountInBuildSettings;

        if (target >= count)
        {
            if (wrapBuildIndex) target = 0;
            else target = Mathf.Clamp(target, 0, count - 1);
        }
        else if (target < 0)
        {
            target = wrapBuildIndex ? (count - 1) : 0;
        }

        SceneManager.LoadScene(target);
    }

    // Helper for realtime waiting
    private static IEnumerator WaitRealtime(float seconds)
    {
        float end = Time.realtimeSinceStartup + Mathf.Max(0f, seconds);
        while (Time.realtimeSinceStartup < end) yield return null;
    }
}
