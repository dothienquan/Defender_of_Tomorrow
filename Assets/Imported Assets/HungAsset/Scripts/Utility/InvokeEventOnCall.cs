using UnityEngine;
using UnityEngine.Events;

public class InvokeEventOnCall : MonoBehaviour {

    public UnityEvent onCallEvent;

    public void CallEvent() {
        onCallEvent.Invoke();
    }
}
