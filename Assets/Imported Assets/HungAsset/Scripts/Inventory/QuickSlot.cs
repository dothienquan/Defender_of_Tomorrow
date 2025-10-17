using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour {

    public Image slotIcon;
    public GameObject slotUseUi;

    public InputActionReference inputReference;
    [HideInInspector]
    public bool slotInputPressed = false;
    private string controlSchemeInUse;

    public ItemObject itemInSlot;

    PlayerInput input;

    private GameObject currentlySelectedItemSlot;
    private QuickSlotManager quickSlotManager;
    private Inventory inventory;

    private void Awake() {
        inventory = Inventory.instance;
        quickSlotManager = QuickSlotManager.instance;
        input = FindObjectOfType<PlayerInput>();
        controlSchemeInUse = input.currentControlScheme;
    }

    void Start() {
        inputReference.action.performed += ctx => {
            if (!slotInputPressed) {
                slotInputPressed = true;
            }
        };

        inputReference.action.canceled += ctx => {
            slotInputPressed = false;
        };
    }

    void OnEnable() {
        InputUser.onChange += OnInputDeviceChange;
    }

    void OnDisable() {
        InputUser.onChange -= OnInputDeviceChange;
    }

    void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) {
            controlSchemeInUse = user.controlScheme.Value.name;
        }
    }
    public void Update() {
        if (inventory.inventoryOpen && currentlySelectedItemSlot) {
            if (slotInputPressed) {
                SetItemInQuickslot(currentlySelectedItemSlot.GetComponent<ItemSlot>().itemInSlot);
                slotInputPressed = false;
            }
        }

        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.name.Contains("ItemSlot")) {
            currentlySelectedItemSlot = EventSystem.current.currentSelectedGameObject;
        }
        else {
            currentlySelectedItemSlot = null;
        }

        if (itemInSlot && !inventory.inventoryOpen) {
            if (slotInputPressed) {
                //Debug.Log("Slot Key <color=yellow>" + slotKey.ToString() + "</color> has been pressed.");
                UseItemInQuickSlot();
                slotInputPressed = false;
            }
        }

    }

    public void UseItemInQuickSlot() {
        //Debug.Log("Use item in <color=yellow>" + gameObject.name + "</color>");
        UseItemFromInventory();

    }

    public void SetItemInQuickslot(ItemObject item) {
        RemoveItemFromQuickslot();

        itemInSlot = item;
        slotIcon.sprite = itemInSlot.icon;
        slotIcon.enabled = true;
        slotUseUi.SetActive(true);
    }

    public void RemoveItemFromQuickslot() {
        itemInSlot = null;
        slotIcon.sprite = null;
        slotIcon.enabled = false;
        slotUseUi.SetActive(false);
    }

    public void UseItemFromInventory() {
        int itemsInInventory = 0;
        int firstItemInInventoryPos = 0;

        for (int i = 0; i < inventory.items.Count; i++) {
            if (inventory.items[i] == itemInSlot) {
                if (itemsInInventory == 0)
                    firstItemInInventoryPos = i;

                itemsInInventory++;
            }
        }

        //Debug.Log("Found <color=green>" + itemsInInventory + " " + itemInSlot.itemName + (itemsInInventory > 1 ? "s" : "") + "</color> in inventory.");

        if (itemsInInventory > 0) {
            if (itemInSlot.itemType == ItemType.Potion) {
                itemInSlot.onUse.Invoke();
                inventory.items.RemoveAt(firstItemInInventoryPos);

                if (itemsInInventory <= 1) {
                    foreach (QuickSlot slot in quickSlotManager.quickSlots) {
                        if (slot.itemInSlot == itemInSlot) {
                            slot.RemoveItemFromQuickslot();
                        }
                    }
                    RemoveItemFromQuickslot();
                }
            }
            else {
                inventory.UseItem(itemInSlot);
            }
        }
    }
}