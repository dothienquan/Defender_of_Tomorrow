using UnityEngine;

public interface IInteractable
{
    Transform PromptAnchor { get; }
    string PromptText { get; }
    void Interact(GameObject interactor);
    void OnFocusEnter();
    void OnFocusExit();
}