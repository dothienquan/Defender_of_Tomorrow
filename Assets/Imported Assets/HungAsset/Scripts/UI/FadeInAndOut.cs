using UnityEngine;

public class FadeInAndOut : MonoBehaviour {

    public Animator animator;

    public void FadeIn() {
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut() {
        animator.SetTrigger("FadeOut");
    }
}
