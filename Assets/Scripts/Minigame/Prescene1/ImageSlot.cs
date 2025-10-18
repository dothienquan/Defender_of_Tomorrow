using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ImageSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image targetImage;

    [Header("Data")]
    [Tooltip("Sprites this slot can cycle through (order defines index)")]
    [SerializeField] private List<Sprite> spritePool = new List<Sprite>();

    [Tooltip("Starting index (0-based)")]
    [SerializeField] private int startIndex = 0;

    [Header("Events")]
    public UnityEvent onChanged;   // <-- NEW: gọi khi slot đổi hình

    public int CurrentIndex { get; private set; } = 0;
    public Sprite CurrentSprite => (spritePool != null && spritePool.Count > 0)
        ? spritePool[CurrentIndex]
        : null;

    public int PoolCount => spritePool?.Count ?? 0;

    private void Awake()
    {
        if (targetImage == null) targetImage = GetComponentInChildren<Image>(true);
        Init(startIndex);
    }

    public void Init(int index)
    {
        if (PoolCount == 0) return;
        CurrentIndex = Mathf.Clamp(index, 0, PoolCount - 1);
        Apply();
    }

    public void Next()
    {
        if (PoolCount == 0) return;
        CurrentIndex = (CurrentIndex + 1) % PoolCount;
        Apply();
    }

    public void Prev()
    {
        if (PoolCount == 0) return;
        CurrentIndex = (CurrentIndex - 1 + PoolCount) % PoolCount;
        Apply();
    }

    public void SetIndex(int index)
    {
        if (PoolCount == 0) return;
        CurrentIndex = Mathf.Clamp(index, 0, PoolCount - 1);
        Apply();
    }

    private void Apply()
    {
        if (targetImage != null) targetImage.sprite = CurrentSprite;
        onChanged?.Invoke(); // <-- NEW: thông báo đã đổi
    }

    // Optional: allow keyboard/controller per-slot (focus the slot, then use up/down)
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) Next();
            if (Input.GetKeyDown(KeyCode.DownArrow)) Prev();
        }
    }
}
