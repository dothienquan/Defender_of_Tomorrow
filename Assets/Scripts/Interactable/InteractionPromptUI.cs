using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    Transform target;

    public void Show(Transform follow, string text)
    {
        target = follow;
        if (label) label.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position;
    }
}