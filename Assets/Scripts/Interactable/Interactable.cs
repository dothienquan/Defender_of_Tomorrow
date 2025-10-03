using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] Transform promptAnchor;     
    [SerializeField] string promptText = "E";

    [Header("On Interact")]
    public UnityEvent onInteract;

    public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;
    public string PromptText => promptText;

    public void Interact(GameObject interactor)
    {
        onInteract?.Invoke();
    }

    public void OnFocusEnter() { /* optional: highlight, sound, etc. */ }
    public void OnFocusExit() { /* optional: unhighlight */ }

    private void Reset()
    {
        if (TryGetComponent<Collider2D>(out var col))
            col.isTrigger = true;
    }
}