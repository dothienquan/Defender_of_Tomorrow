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
    [Tooltip("If true, checks automatically whenever any slot changes (call Check() from slot events if needed).")]
    public bool autoCheckOnChange = false;

    private void Start()
    {
        // Optional: auto-check when scene loads
        // Check();
    }

    // Call this from a "Submit" button, or wire slots to call Check() after Next/Prev if autoCheckOnChange=true
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
                if (slots[i].CurrentIndex != secretIndices[i])
                {
                    match = false;
                    break;
                }
            }
        }
        else // usingSprites
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var s = slots[i].CurrentSprite;
                if (s != secretSprites[i])
                {
                    match = false;
                    break;
                }
            }
        }

        if (match) onCorrect?.Invoke();
        else onIncorrect?.Invoke();
    }

    // Optional helper: set all indices at once (e.g., randomize start)
    public void SetAllIndices(int[] indices)
    {
        if (indices == null || indices.Length != slots.Count) return;
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetIndex(indices[i]);
    }
}
