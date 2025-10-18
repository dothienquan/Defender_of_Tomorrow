using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImageCodeLock : MonoBehaviour
{
    [Header("Slots (left→right)")]
    [SerializeField] private List<ImageSlot> slots = new List<ImageSlot>();

    [Header("Secret by Index (optional)")]
    [Tooltip("Leave empty if using Secret by Sprites instead.")]
    [SerializeField] private List<int> secretIndices = new List<int>(); // length should match slots

    [Header("Secret by Sprites (optional)")]
    [Tooltip("Leave empty if using Secret by Index instead.")]
    [SerializeField] private List<Sprite> secretSprites = new List<Sprite>(); // length should match slots

    [Header("Events")]
    public UnityEvent onCorrect;
    public UnityEvent onIncorrect;

    [Header("Options")]
    [Tooltip("If true, checks automatically whenever any slot changes.")]
    public bool autoCheckOnChange = true;

    [Tooltip("Instantiate this VFX when the code is correct (optional).")]
    [SerializeField] private GameObject correctVFXPrefab;

    [Tooltip("Where to spawn the VFX (defaults to this object).")]
    [SerializeField] private Transform vfxSpawnAt;

    [Tooltip("Auto destroy spawned VFX after seconds (<=0 = don't destroy).")]
    [SerializeField] private float vfxLifetime = 2f;

    [Tooltip("Disable all slots after correct?")]
    [SerializeField] private bool disableSlotsOnCorrect = true;

    private void OnEnable()
    {
        // Subscribe to slot change -> auto check
        if (autoCheckOnChange && slots != null)
        {
            foreach (var s in slots)
                if (s != null) s.onChanged.AddListener(OnAnySlotChanged);
        }
    }

    private void OnDisable()
    {
        if (slots != null)
        {
            foreach (var s in slots)
                if (s != null) s.onChanged.RemoveListener(OnAnySlotChanged);
        }
    }

    private void OnAnySlotChanged()
    {
        if (autoCheckOnChange) Check();
    }

    public void Check()
    {
        if (slots == null || slots.Count == 0)
        {
            Debug.LogWarning("[ImageCodeLock] No slots assigned.");
            return;
        }

        bool usingIndices = secretIndices != null && secretIndices.Count == slots.Count;
        bool usingSprites = secretSprites != null && secretSprites.Count == slots.Count;

        if (!usingIndices && !usingSprites)
        {
            Debug.LogWarning("[ImageCodeLock] Secret not set or size mismatch.");
            onIncorrect?.Invoke();
            return;
        }

        bool match = true;

        if (usingIndices)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].CurrentIndex != secretIndices[i]) { match = false; break; }
            }
        }
        else // usingSprites
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].CurrentSprite != secretSprites[i]) { match = false; break; }
            }
        }

        if (match)
        {
            onCorrect?.Invoke();
            PlayCorrectVFX();

            if (disableSlotsOnCorrect)
            {
                foreach (var s in slots) if (s) s.enabled = false;
                enabled = false; // stop further checks if muốn
            }
        }
        else
        {
            onIncorrect?.Invoke();
        }
    }

    private void PlayCorrectVFX()
    {
        if (correctVFXPrefab == null) return;

        Transform where = vfxSpawnAt != null ? vfxSpawnAt : transform;
        var vfx = Instantiate(correctVFXPrefab, where.position, where.rotation, where);

        if (vfxLifetime > 0f)
            Destroy(vfx, vfxLifetime);
    }

    // Optional helper: set all indices at once (e.g., randomize start)
    public void SetAllIndices(int[] indices)
    {
        if (indices == null || indices.Length != slots.Count) return;
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetIndex(indices[i]);
    }
}
