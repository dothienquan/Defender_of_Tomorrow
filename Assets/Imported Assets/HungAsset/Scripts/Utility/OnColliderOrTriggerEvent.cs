using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnColliderOrTriggerEvent : MonoBehaviour {
    public string compareTag;
    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionExit;
    public UnityEvent onCollisionStay;
    public UnityEvent onTriggerEnterEvent;
    public UnityEvent onTriggerExitEvent;
    public UnityEvent onTriggerStayEvent;

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag(compareTag)) {
            onTriggerEnterEvent.Invoke();
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(compareTag)) {
            onTriggerExitEvent.Invoke();
        }
    }

    public void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(compareTag)) {
            onTriggerStayEvent.Invoke();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(compareTag)) {
            onCollisionEnter.Invoke();
        }
    }
    public void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(compareTag)) {
            onCollisionExit.Invoke();
        }
    }
    public void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(compareTag)) {
            onCollisionStay.Invoke();
        }
    }
}
