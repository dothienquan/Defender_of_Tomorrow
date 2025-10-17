using UnityEngine;
using UnityEngine.Events;

public enum ItemType {
    None = 0,
    Potion = 1,
    MeleeWeapon = 2,
    RangedWeapon = 3,
    Equipment = 4,
}

[CreateAssetMenu(fileName = "New Item", menuName = "2D Game System/Create New Item")]
public class ItemObject : ScriptableObject {

    public int id;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    [TextArea(2, 6)]
    public string itemDescription = "";
    public string valueType;
    [Tooltip("Item Value will be used for:\n" +
        "Item Type Melee/Ranged Weapon - for damage\n" +
        "Item Type Potion - for add/remove health")]
    public int itemValue;

    public UnityEvent onUse;
    public UnityEvent onUnequip;

    public void EnableSlide() {
        Player.instance.canSlide = true;
    }

    public void DisableSlide() {
        Player.instance.canSlide = false;
    }

    public void EnableJump() {
        Player.instance.canJump = true;
    }

    public void DisableJump() {
        Player.instance.canJump = false;
    }

    public void EnableSword() {
        Player.instance.canUseSword = true;
        Player.instance.damage = itemValue;
    }

    public void DisableSword() {
        Player.instance.canUseSword = false;
        Player.instance.damage = 0;
    }

    public void EnableBow() {
        Player.instance.canUseBow = true;
        Player.instance.arrowDamage = itemValue;
    }

    public void DisableBow() {
        Player.instance.canUseBow = false;
        Player.instance.arrowDamage = 0;
    }

    public void HealthSet() {
        Player.instance.currentHealth = itemValue;
    }

    public void HealthAdd() {
        if (Player.instance.isInvincible == false)
            Player.instance.currentHealth += itemValue;
    }

    public void HealthRemove() {
        if (Player.instance.isInvincible == false)
            Player.instance.currentHealth -= itemValue;
    }
    public void EnableInvincibility() {
        Player.instance.isInvincible = true;
    }
    public void DisableInvincibility() {
        Player.instance.isInvincible = false;
    }

    public void PlaySound(AudioClip clip) {
        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), Player.instance.transform.position, Quaternion.identity) as GameObject;
        AudioSource aSource = sfx.GetComponent<AudioSource>();
        aSource.spatialBlend = 0f;

        if (clip != null) {
            aSource.clip = clip;
            aSource.Play();

            Destroy(sfx, (clip.length + 0.05f));
        }
        else {
            Destroy(sfx);
        }
    }
}