using UnityEngine;
using UnityEngine.UI;

public class AutoImageStyle : MonoBehaviour
{
    [SerializeField] private Image uiImage;

    void Start()
    {
        if (uiImage == null)
            uiImage = GetComponent<Image>();

        // Just in case the sprite is assigned late, run this after it’s set
        if (uiImage != null && uiImage.sprite != null)
        {
            ApplyImageStyle();
        }
    }

    public void ApplyImageStyle()
    {
        // Keep image's original ratio
        uiImage.type = Image.Type.Simple;
        uiImage.preserveAspect = true;
    }
}
