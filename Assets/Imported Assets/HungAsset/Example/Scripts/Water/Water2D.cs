using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Water2D : MonoBehaviour {

    public float waterSize = 1.0f;

    public SpriteRenderer sr;
    public Camera cam;

    private void Update() {
        if(sr != null && cam != null) {
            cam.orthographicSize = waterSize;
            sr.transform.localScale = new Vector2(waterSize * 25.0f, waterSize * 12.50f);
        }
    }
}
