using UnityEngine;

public class SetupWater2D : MonoBehaviour {

    private RenderTexture render_text_ref;
    private Material water_mat_ref;

    public void SetupWater() {
        water_mat_ref = Resources.Load<Material>("DefaultWaterMaterial");
        render_text_ref = new RenderTexture(512, 512, 16);

        GameObject water = new GameObject("Water2D");
        water.transform.localScale = new Vector2(25.0f, 12.50f);

        GameObject camera = new GameObject("Camera");
        camera.transform.parent = water.transform;
        camera.transform.localScale = new Vector2(1.0f, 1.0f);
        Camera cam = camera.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 1.0f;
        cam.targetTexture = render_text_ref;

        SpriteRenderer water_sr = water.AddComponent<SpriteRenderer>();

        water_sr.material = water_mat_ref;
    }
}
