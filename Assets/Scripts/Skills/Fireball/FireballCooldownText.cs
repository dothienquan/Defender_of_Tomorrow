using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireballCooldownText : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private PlayerFireball fireball;

    [Header("Target (either is fine)")]
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Text uiText;

    public enum ReadyDisplay { ReadyText, Zero, Hide }
    public enum Rounding { Round, Ceil, Floor }

    [Header("Display")]
    [SerializeField] private ReadyDisplay whenReady = ReadyDisplay.ReadyText;
    [SerializeField] private string readyText = "Ready";
    [SerializeField, Range(0, 3)] private int decimals = 1;
    [SerializeField] private Rounding rounding = Rounding.Round;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "s";

    void Reset()
    {
        // Auto-wire common cases
        tmp = GetComponent<TextMeshProUGUI>();
        uiText = GetComponent<Text>();
        // Try to auto-find a PlayerFireball on the same object or parent
        if (!fireball)
        {
            fireball = GetComponentInParent<PlayerFireball>();
        }
    }

    void Update()
    {
        if (!fireball) return;

        if (!fireball.OnCooldown)
        {
            switch (whenReady)
            {
                case ReadyDisplay.Hide:
                    SetActive(false);
                    return;
                case ReadyDisplay.Zero:
                    SetActive(true);
                    SetText(BuildNumberString(0f));
                    return;
                default:
                    SetActive(true);
                    SetText(readyText);
                    return;
            }
        }

        // cooling down
        SetActive(true);
        float seconds = fireball.CooldownRemaining;
        SetText(BuildNumberString(seconds));
    }

    string BuildNumberString(float seconds)
    {
        float scale = Mathf.Pow(10f, decimals);
        switch (rounding)
        {
            case Rounding.Ceil: seconds = Mathf.Ceil(seconds * scale) / scale; break;
            case Rounding.Floor: seconds = Mathf.Floor(seconds * scale) / scale; break;
            default: seconds = Mathf.Round(seconds * scale) / scale; break;
        }

        string fmt = "0" + (decimals > 0 ? "." + new string('0', decimals) : "");
        return $"{prefix}{seconds.ToString(fmt)}{suffix}";
    }

    void SetText(string s)
    {
        if (tmp) tmp.text = s;
        if (uiText) uiText.text = s;
    }

    void SetActive(bool active)
    {
        if (tmp) tmp.gameObject.SetActive(active);
        if (uiText) uiText.gameObject.SetActive(active);
        if (!tmp && !uiText) gameObject.SetActive(active);
    }
}
