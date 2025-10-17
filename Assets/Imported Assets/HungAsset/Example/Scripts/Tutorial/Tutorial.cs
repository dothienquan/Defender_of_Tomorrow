using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialType { OnStay = 0, OnEnter = 1}

public class Tutorial : MonoBehaviour {

    public TutorialType tutorialType;

    public UnityEvent OnTrigger;
    private bool eventInvoked = false;

    public float alphaSpeed;
    private bool showUi;

    public CanvasGroup tutCanvasGroup;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(tutorialType == TutorialType.OnEnter) {
            if (collision.gameObject.CompareTag("Player")) {
                showUi = true;

                if (!eventInvoked) {
                    OnTrigger.Invoke();
                    eventInvoked = true;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (tutorialType == TutorialType.OnStay) {
            if (collision.gameObject.CompareTag("Player")) {
                showUi = true;

                if (!eventInvoked) {
                    OnTrigger.Invoke();
                    eventInvoked = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            showUi = false;
        }
    }

    public void Update() {
        if(showUi) {
            StartCoroutine(ShowUI());
        }
        else {
            StartCoroutine(HideUI());
        }
    }

    private IEnumerator ShowUI() {
        yield return new WaitForSeconds(0.15f);

        if (tutCanvasGroup.alpha < 1f) {
            tutCanvasGroup.alpha += Time.deltaTime * alphaSpeed;
        }
    }
    private IEnumerator HideUI() {
        yield return new WaitForSeconds(0.15f);

        if (tutCanvasGroup.alpha > 0f) {
            tutCanvasGroup.alpha -= Time.deltaTime * alphaSpeed;
        }
    }
}
