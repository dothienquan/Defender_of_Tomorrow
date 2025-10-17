using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour {

    [Header("ITEM TO PICKUP")]
    public ItemObject item;
    [Space]
    [Header("EVENTS")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onPickup;
    public bool destroyOnPickup = false;

    private bool inTrigger = false;

    public void PickupItem() {

        Inventory.instance.AddItem(item);
        onPickup.Invoke();

        if (destroyOnPickup == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            inTrigger = true;
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            inTrigger = false;
            onTriggerExit.Invoke();
        }
    }

    private void Update() {
        if (Player.instance.use == 1f) {
            if (inTrigger == true) {
                PickupItem();
            }
            StartCoroutine(UseActionFix());
        }
    }

    IEnumerator UseActionFix() {
        yield return new WaitForEndOfFrame();
        Player.instance.use = 0f;
    }
}
