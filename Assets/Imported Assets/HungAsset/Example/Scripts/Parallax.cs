using UnityEngine;

[ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
    
    [Range(0.01f,10f)] public float scrollSpeed = .3f;
    private GameObject parCamera; // cam
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float yMultiplier = 1f;

    // Start is called before the first frame update
    void Start() {
        parCamera = Camera.main.gameObject;
       
    }

    // Update is called once per frame
    void FixedUpdate() {
        float newXpos = parCamera.transform.position.x * scrollSpeed;
        float newYpos = (parCamera.transform.position.y+yOffset) * (scrollSpeed*yMultiplier);

        transform.position = new Vector3(newXpos, newYpos, transform.position.z);
    }

}
