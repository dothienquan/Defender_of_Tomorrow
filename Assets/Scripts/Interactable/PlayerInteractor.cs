
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference interactAction; 

    [Header("UI")]
    public InteractionPromptUI promptUI;

    IInteractable current;

    void OnEnable()
    {
        if (interactAction) interactAction.action.Enable();
    }
    void OnDisable()
    {
        if (interactAction) interactAction.action.Disable();
    }

    void Update()
    {
        if (current != null && interactAction && interactAction.action.WasPerformedThisFrame())
        {
            current.Interact(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            current = interactable;
            current.OnFocusEnter();
            if (promptUI) promptUI.Show(current.PromptAnchor, current.PromptText);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (current != null && other.TryGetComponent<IInteractable>(out var interactable) && interactable == current)
        {
            current.OnFocusExit();
            current = null;
            if (promptUI) promptUI.Hide();
        }
    }
}
