using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    [Header("ITEMS IN INVENTORY")]
    [Tooltip("Here we store items that player character has picked up")]
    public List<ItemObject> items = new List<ItemObject>();
    private List<GameObject> itemSlots = new List<GameObject>();
    [Space]
    [Header("CURRENTLY EQUIPPED ITEMS")]
    public ItemObject equippedMeleeWeapon;
    private bool meleeWeaponSlotHighlighted = false;
    public ItemObject equippedRangedWeapon;
    private bool rangedWeaponSlotHighlighted = false;
    public ItemObject equippedEquipment;
    private bool equipmentSlotHighlighted = false;
    [Space]
    [Header("EVENTS ON OPEN/CLOSE")]
    public UnityEvent onInventoryOpen;
    public UnityEvent onInventoryClose;
    [HideInInspector]
    public bool inventoryOpen = false;

    public static Inventory instance;
    [Space]
    [Header("INTERNAL COMPONENTS")]
    public Transform itemContent;
    public GameObject inventoryItem;
    public Image itemDescIcon;
    public TextMeshProUGUI itemDescName;
    public TextMeshProUGUI itemDescValue;
    public TextMeshProUGUI itemDescText;

    private List<InputActionReference> quickslotsKeys = new List<InputActionReference>();

    Animator animator;
    CanvasGroup canvasGroup;

    private Player player;
    private QuickSlotManager quickslotManager;

    private void Awake() {
        instance = this;

        animator = GetComponent<Animator>();

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        inventoryOpen = false;
    }

    private void Start() {
        player = Player.instance;
        quickslotManager = QuickSlotManager.instance;

        for (int i = 0; i < quickslotManager.quickSlots.Count; i++) {
            quickslotsKeys.Add(quickslotManager.quickSlots[i].inputReference);
        }
    }

    public void AddItem(ItemObject item) {
        items.Add(item);
    }

    public void RemoveItem(ItemObject item) {
        items.Remove(item);
    }

    public void ListItem() {
        if (items.Count > 0) {
            //Then we are instantiating item slot prefabs and setting their items
            for (int i = 0; i < items.Count; i++) {
                GameObject obj = Instantiate(inventoryItem, itemContent);
                itemSlots.Add(obj);

                var itemSlot = obj.GetComponent<ItemSlot>();

                itemSlot.itemInSlot = items[i];
                itemSlot.itemIconImage.sprite = items[i].icon;

                switch (items[i].itemType) {
                    case ItemType.MeleeWeapon:
                        if (items[i] == equippedMeleeWeapon && meleeWeaponSlotHighlighted == false) {
                            itemSlot.itemEquippedImage.enabled = true;
                            meleeWeaponSlotHighlighted = true;
                        }
                        else {
                            itemSlot.itemEquippedImage.enabled = false;
                        }
                        break;
                    case ItemType.RangedWeapon:
                        if (items[i] == equippedRangedWeapon && rangedWeaponSlotHighlighted == false) {
                            itemSlot.itemEquippedImage.enabled = true;
                            rangedWeaponSlotHighlighted = true;
                        }
                        else {
                            itemSlot.itemEquippedImage.enabled = false;
                        }
                        break;
                    case ItemType.Equipment:
                        if (items[i] == equippedEquipment && equipmentSlotHighlighted == false) {
                            itemSlot.itemEquippedImage.enabled = true;
                            equipmentSlotHighlighted = true;
                        }
                        else {
                            itemSlot.itemEquippedImage.enabled = false;
                        }
                        break;
                }
            }

            DisplayDescription(items[0]);
            SetFirstSelectedSlot();
        }
    }

    public void DestroyItemList() {
        //First we are cleaning list of Items before opening inventory

        for (int i = 0; i < itemSlots.Count; i++) {
            Destroy(itemSlots[i]);
        }

        itemSlots = new List<GameObject>();

        meleeWeaponSlotHighlighted = false;
        rangedWeaponSlotHighlighted = false;
        equipmentSlotHighlighted = false;
    }

    /// <summary>
    /// Display description when item is selected in inventory
    /// </summary>
    /// <param name="item"></param>
    public void DisplayDescription(ItemObject item) {
        if (item != null) {
            if (item.icon != null) {
                itemDescIcon.sprite = item.icon;
                animator.Play("InventoryWindowDescriptionShow"); //We will show description only if item has icon. That is the only prerequisite
            }
            else {
                animator.Play("InventoryWindowDescriptionHide");
            }

            if (item.itemName != string.Empty)
                itemDescName.text = item.itemName;
            else
                animator.Play("InventoryWindowDescriptionHide");

            if (item.valueType != string.Empty) {
                if (item.itemValue != 0)
                    itemDescValue.text = item.valueType + item.itemValue;
                else
                    itemDescValue.text = item.valueType;
            }
            else {
                animator.Play("InventoryWindowDescriptionHide");
            }

            if (item.itemDescription != string.Empty)
                itemDescText.text = item.itemDescription;
            else
                animator.Play("InventoryWindowDescriptionHide");
        }
        else {
            animator.Play("InventoryWindowDescriptionHide");
        }
    }

    public void LateUpdate() {
        if (player.inventory == 1f) {
            InventoryCanvasState();
            player.inventory = 0f;
        }
    }

    private void InventoryCanvasState() {
        if (canvasGroup.alpha == 1) {
            animator.Play("InventoryWindowClose");
            onInventoryClose.Invoke();
            inventoryOpen = false;
            DestroyItemList();
        }
        else if (canvasGroup.alpha == 0) {
            animator.Play("InventoryWindowOpen");
            onInventoryOpen.Invoke();
            inventoryOpen = true;
            ListItem();
        }
    }

    public void SetFirstSelectedSlot() {
        if (itemSlots.Count > 0) {
            for (int i = 0; i < itemSlots.Count; i++) {
                if (itemSlots[i]) {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(itemSlots[i]);
                    break;
                }

            }
        }
    }

    public void UseItem(ItemObject itemObject) {
        switch (itemObject.itemType) {
            case ItemType.MeleeWeapon:
                if (equippedMeleeWeapon != null) {
                    if (equippedMeleeWeapon != itemObject) {
                        equippedMeleeWeapon.onUnequip.Invoke();
                        itemObject.onUse.Invoke();
                        equippedMeleeWeapon = itemObject;
                    }
                    else {
                        itemObject.onUnequip.Invoke();
                        equippedMeleeWeapon = null;
                    }
                }
                else {
                    itemObject.onUse.Invoke();
                    equippedMeleeWeapon = itemObject;
                }
                break;
            case ItemType.RangedWeapon:
                if (equippedRangedWeapon != null) {
                    if (equippedRangedWeapon != itemObject) {
                        equippedRangedWeapon.onUnequip.Invoke();
                        itemObject.onUse.Invoke();
                        equippedRangedWeapon = itemObject;
                    }
                    else {
                        itemObject.onUnequip.Invoke();
                        equippedRangedWeapon = null;
                    }
                }
                else {
                    itemObject.onUse.Invoke();
                    equippedRangedWeapon = itemObject;
                }
                break;
            case ItemType.Equipment:
                if (equippedEquipment != null) {
                    if (equippedEquipment != itemObject) {
                        equippedEquipment.onUnequip.Invoke();
                        itemObject.onUse.Invoke();
                        equippedEquipment = itemObject;
                    }
                    else {
                        itemObject.onUnequip.Invoke();
                        equippedEquipment = null;
                    }
                }
                else {
                    itemObject.onUse.Invoke();
                    equippedEquipment = itemObject;
                }
                break;
            case ItemType.Potion:
                break;
            case ItemType.None:
                break;
        }
    }
}