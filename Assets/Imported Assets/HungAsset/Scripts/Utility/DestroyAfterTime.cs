using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float destroyTime;

    public void Update() {
        Destroy(gameObject, destroyTime);
    }
}
