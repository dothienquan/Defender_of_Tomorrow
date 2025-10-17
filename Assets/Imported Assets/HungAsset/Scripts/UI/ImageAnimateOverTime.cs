using System.Collections;
using UnityEngine;

public enum ImageAnimateType { ChangeColor = 0, ChangeAlpha = 1}

public class ImageAnimateOverTime : MonoBehaviour {

    public float time = 5.0f;
    public float switchTime = 2.0f;
    public ImageAnimateType animationType;

    private CanvasGroup cg;

    private bool done = false;

    public void Start() {
        if(animationType == ImageAnimateType.ChangeAlpha) {
            if(!GetComponent<CanvasGroup>()) {
                cg = gameObject.AddComponent<CanvasGroup>();
            }
            else {
                cg = GetComponent<CanvasGroup>();
            }

            cg.alpha = 1f;
        }
    }

    public void Update() {
        if (done == false) {
            StartCoroutine(ImageAnimateOn());
        }
        else {
            StartCoroutine(ImageAnimateOff());
        }
    }

    IEnumerator ImageAnimateOn() {

        yield return new WaitForSeconds(time);

        if(animationType == ImageAnimateType.ChangeAlpha) {

            if(cg.alpha > 0f) {
                cg.alpha -= Time.deltaTime * switchTime;
            }
            else {
                done = true;
            }
        }
        else if (animationType == ImageAnimateType.ChangeColor) {

        }
    }

    IEnumerator ImageAnimateOff() {

        yield return new WaitForSeconds(time);

        if (animationType == ImageAnimateType.ChangeAlpha) {

            if (cg.alpha < 1f) {
                cg.alpha += Time.deltaTime * switchTime;
            }
            else {
                done = false;
            }
        }
        else if (animationType == ImageAnimateType.ChangeColor) {

        }
    }
}
