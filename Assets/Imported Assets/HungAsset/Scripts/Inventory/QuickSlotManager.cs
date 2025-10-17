using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour {

    public List<QuickSlot> quickSlots = new List<QuickSlot>();

    private Animator animator;


    public static QuickSlotManager instance;

    private void Awake() {
        instance = this;

        animator = GetComponent<Animator>();

        var temp = GetComponentsInChildren<QuickSlot>();
        quickSlots.AddRange(temp);
    }

    private void Start() {
        QuickSlotCanvasState(1);
    }

    private void QuickSlotCanvasState(int state) {
        if (state == 0) {
            animator.Play("QuickSlotHide");
        }
        else if (state == 1) {
            animator.Play("QuickSlotShow");
        }
    }
}
