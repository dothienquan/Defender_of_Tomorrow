// PortalSuckIn.cs
// Put this on your Portal (needs a Collider2D set to isTrigger).
// When the Player enters, tween them into the portal center (move root, scale/spin/fade visuals).
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // <-- NEW

[RequireComponent(typeof(Collider2D))]
public class PortalSuckIn : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool oneUse = false;
    private bool used = false;

    [Header("Tween Target (player)")]
    [Tooltip("Assign the player's visual root (child with SpriteRenderer/Animator). If null, falls back to whole player.")]
    [SerializeField] private Transform playerVisualRoot;

    [Header("Tween Settings")]
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private Ease moveEase = Ease.InCubic;
    [SerializeField] private float endScale = 0.0f;
    [SerializeField] private int spinTurns = 2;
    [SerializeField] private bool ignoreTimeScale = true;
    [SerializeField] private bool fade = true;

    [Header("Control/Physics Handling")]
    [Tooltip("Drop your movement/controller scripts here to disable during suction.")]
    [SerializeField] private List<MonoBehaviour> playerComponentsToDisable = new List<MonoBehaviour>();
    [SerializeField] private bool disablePhysicsDuringTween = true;

    [Header("Optional Camera Handling")]
    [Tooltip("Disable your camera follow component during tween (your follow script or a CinemachineVirtualCamera).")]
    [SerializeField] private Behaviour cameraFollowComponent;
    [SerializeField] private bool disableCameraFollowDuringTween = true;

    [Header("Events")]
    public UnityEvent onSuckStart;
    public UnityEvent onSuckComplete;

    // ---------------- NEW: Scene switch options ----------------
    [Header("Next Scene (optional)")]
    [Tooltip("Leave empty to skip auto-loading by name.")]
    [SerializeField] private string nextSceneName = "";
    [Tooltip("Use -1 to skip auto-loading by index.")]
    [SerializeField] private int nextSceneBuildIndex = -1;
    [Tooltip("Delay after animation completes before loading scene.")]
    [SerializeField] private float loadDelay = 0f;
    [Tooltip("Load scene additive? If false, loads Single.")]
    [SerializeField] private bool loadAdditive = false;
    // -----------------------------------------------------------

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used && oneUse) return;
        if (!other.CompareTag(playerTag)) return;

        SuckIn(other.gameObject);
        if (oneUse) used = true;
    }

    public void SuckIn(GameObject player)
    {
        if (!player) return;

        onSuckStart?.Invoke();

        // Animate only the visual child so camera (parented to player) isn't scaled/rotated.
        Transform visual = playerVisualRoot != null ? playerVisualRoot : player.transform;

        var rb = player.GetComponent<Rigidbody2D>();
        var sr = visual.GetComponent<SpriteRenderer>();
        var cg = visual.GetComponent<CanvasGroup>();

        // Disable controls
        foreach (var comp in playerComponentsToDisable)
            if (comp) comp.enabled = false;

        // Disable camera follow (optional)
        bool camWasEnabled = false;
        if (disableCameraFollowDuringTween && cameraFollowComponent != null)
        {
            camWasEnabled = cameraFollowComponent.enabled;
            cameraFollowComponent.enabled = false;
        }

        // Pause physics if desired
        bool rbHadSim = false;
        Vector2 rbVel = Vector2.zero;
        float rbGrav = 0f;
        if (rb && disablePhysicsDuringTween)
        {
            rbHadSim = rb.simulated;
            rbVel = rb.linearVelocity;
            rbGrav = rb.gravityScale;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        var seq = DOTween.Sequence().SetUpdate(ignoreTimeScale);

        // Move player root to portal center
        seq.Join(player.transform.DOMove(transform.position, duration).SetEase(moveEase));
        // Scale/rotate/fade only visuals
        seq.Join(visual.DOScale(Vector3.one * endScale, duration).SetEase(moveEase));
        if (spinTurns != 0)
            seq.Join(visual.DORotate(new Vector3(0, 0, 360f * spinTurns), duration, RotateMode.FastBeyond360));
        if (fade)
        {
            if (cg) seq.Join(cg.DOFade(0f, duration));
            else if (sr) seq.Join(sr.DOFade(0f, duration));
        }

        seq.OnComplete(() =>
        {
            if (rb && disablePhysicsDuringTween)
            {
                rb.simulated = rbHadSim;
                rb.linearVelocity = rbVel;
                rb.gravityScale = rbGrav;
            }

            if (disableCameraFollowDuringTween && cameraFollowComponent != null)
                cameraFollowComponent.enabled = camWasEnabled;

            onSuckComplete?.Invoke();

            // NEW: Load next scene after animation
            TryLoadNextScene();
        });
    }

    // ---------------- NEW ----------------
    private void TryLoadNextScene()
    {
        // If neither name nor index set, do nothing
        bool hasName = !string.IsNullOrEmpty(nextSceneName);
        bool hasIndex = nextSceneBuildIndex >= 0;

        if (!hasName && !hasIndex) return;
        StartCoroutine(CoLoadAfterDelay());
    }

    private IEnumerator CoLoadAfterDelay()
    {
        if (loadDelay > 0f)
        {
            // respect time scale choice: if tween used ignoreTimeScale, we should wait unscaled
            float t = 0f;
            while (t < loadDelay)
            {
                t += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        var mode = loadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName, mode);
        else
            SceneManager.LoadScene(nextSceneBuildIndex, mode);
    }
    // -------------------------------------
}
