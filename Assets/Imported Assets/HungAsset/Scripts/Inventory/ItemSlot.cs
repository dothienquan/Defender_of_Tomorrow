using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, ISelectHandler {

    public ItemObject itemInSlot;

    public Image itemIconImage;
    public Image itemEquippedImage;

    public int itemsIndexInList = 0;

    private Inventory inventory;

    private void Start() {
        inventory = Inventory.instance;
    }

    public void UseItem() {
        switch (itemInSlot.itemType) {
            case ItemType.MeleeWeapon:
                if (inventory.equippedMeleeWeapon != null) {
                    if (inventory.equippedMeleeWeapon != itemInSlot) {
                        inventory.equippedMeleeWeapon.onUnequip.Invoke();
                        foreach (Transform item in inventory.itemContent) {
                            if (item.GetComponent<ItemSlot>().itemInSlot == inventory.equippedMeleeWeapon) {
                                item.GetComponent<ItemSlot>().itemEquippedImage.enabled = false;
                                break;
                            }
                        }
                        itemEquippedImage.enabled = true;
                        itemInSlot.onUse.Invoke();
                        inventory.equippedMeleeWeapon = itemInSlot;
                    }
                    else {
                        itemEquippedImage.enabled = false;
                        itemInSlot.onUnequip.Invoke();
                        inventory.equippedMeleeWeapon = null;
                    }
                }
                else {
                    itemEquippedImage.enabled = true;
                    itemInSlot.onUse.Invoke();
                    inventory.equippedMeleeWeapon = itemInSlot;
                }
                break;
            case ItemType.RangedWeapon:
                if (inventory.equippedRangedWeapon != null) {
                    if (inventory.equippedRangedWeapon != itemInSlot) {
                        inventory.equippedRangedWeapon.onUnequip.Invoke();
                        foreach (Transform item in inventory.itemContent) {
                            if (item.GetComponent<ItemSlot>().itemInSlot == inventory.equippedRangedWeapon) {
                                item.GetComponent<ItemSlot>().itemEquippedImage.enabled = false;
                                break;
                            }
                        }
                        itemEquippedImage.enabled = true;
                        itemInSlot.onUse.Invoke();
                        inventory.equippedRangedWeapon = itemInSlot;
                    }
                    else {
                        itemEquippedImage.enabled = false;
                        itemInSlot.onUnequip.Invoke();
                        inventory.equippedRangedWeapon = null;
                    }
                }
                else {
                    itemEquippedImage.enabled = true;
                    itemInSlot.onUse.Invoke();
                    inventory.equippedRangedWeapon = itemInSlot;
                }
                break;
            case ItemType.Equipment:
                if (inventory.equippedEquipment != null) {
                    if (inventory.equippedEquipment != itemInSlot) {
                        inventory.equippedEquipment.onUnequip.Invoke();
                        foreach (Transform item in inventory.itemContent) {
                            if (item.GetComponent<ItemSlot>().itemInSlot == inventory.equippedEquipment) {
                                item.GetComponent<ItemSlot>().itemEquippedImage.enabled = false;
                                break;
                            }
                        }
                        itemEquippedImage.enabled = true;
                        itemInSlot.onUse.Invoke();
                        inventory.equippedEquipment = itemInSlot;
                    }
                    else {
                        itemEquippedImage.enabled = false;
                        itemInSlot.onUnequip.Invoke();
                        inventory.equippedEquipment = null;
                    }
                }
                else {
                    itemEquippedImage.enabled = true;
                    itemInSlot.onUse.Invoke();
                    inventory.equippedEquipment = itemInSlot;
                }
                break;
            case ItemType.Potion:
                itemEquippedImage.enabled = false;
                itemInSlot.onUse.Invoke();
                inventory.RemoveItem(itemInSlot);
                inventory.DestroyItemList();
                inventory.ListItem();
                break;
            case ItemType.None:
                break;
        }
    }

    public void OnSelect(BaseEventData eventData) {
        if (inventory != null) {
            inventory.DisplayDescription(itemInSlot);
        }
    }

    public void LateUpdate() {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject) {

        }
    }
}
