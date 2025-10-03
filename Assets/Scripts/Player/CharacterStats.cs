using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;
    public int maxMana = 50;

    [SerializeField] private int currentHP;
    [SerializeField] private int currentMana;

    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;

    [Header("Behavior")]
    [Tooltip("If true, dragging the sliders will change HP/Mana. If false, sliders are display-only.")]
    public bool uiControlsStats = false;

    private void Awake()
    {
        // Init runtime values
        currentHP = Mathf.Clamp(currentHP == 0 ? maxHP : currentHP, 0, maxHP);
        currentMana = Mathf.Clamp(currentMana == 0 ? maxMana : currentMana, 0, maxMana);

        // Init sliders
        if (hpSlider)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.wholeNumbers = true;
            hpSlider.value = currentHP;

            hpSlider.interactable = uiControlsStats;
            if (uiControlsStats) hpSlider.onValueChanged.AddListener(OnHPSliderChanged);
        }

        if (manaSlider)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.wholeNumbers = true;
            manaSlider.value = currentMana;

            manaSlider.interactable = uiControlsStats;
            if (uiControlsStats) manaSlider.onValueChanged.AddListener(OnManaSliderChanged);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    // === Public API (for gameplay) ===
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - Mathf.Abs(damage), 0, maxHP);
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + Mathf.Abs(amount), 0, maxHP);
        UpdateUI();
    }

    public void UseMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana - Mathf.Abs(amount), 0, maxMana);
        UpdateUI();
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana + Mathf.Abs(amount), 0, maxMana);
        UpdateUI();
    }

    // === Slider callbacks (only used when uiControlsStats = true) ===
    private void OnHPSliderChanged(float value)
    {
        currentHP = Mathf.Clamp(Mathf.RoundToInt(value), 0, maxHP);
        UpdateTextsOnly();
    }

    private void OnManaSliderChanged(float value)
    {
        currentMana = Mathf.Clamp(Mathf.RoundToInt(value), 0, maxMana);
        UpdateTextsOnly();
    }

    // === UI Update ===
    private void UpdateUI()
    {
        if (hpSlider) hpSlider.value = currentHP;
        if (manaSlider) manaSlider.value = currentMana;
        UpdateTextsOnly();
    }

    private void UpdateTextsOnly()
    {
        if (hpText) hpText.text = $"{currentHP} / {maxHP}";
        if (manaText) manaText.text = $"{currentMana} / {maxMana}";
    }
}
