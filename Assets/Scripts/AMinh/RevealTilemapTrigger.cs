using UnityEngine;
using UnityEngine.Tilemaps;

public class RevealTilemapTrigger : MonoBehaviour
{
    public Tilemap tilemap; // đổi từ TilemapRenderer sang Tilemap
    public TilemapCollider2D tilemapCollider;

    private void Start()
    {
        if (tilemap != null)
            tilemap.color = new Color(1, 1, 1, 0.3f); // mờ ban đầu

        if (tilemapCollider != null)
            tilemapCollider.enabled = false; // không thể đứng lên ban đầu
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // khi Player chạm vào vùng trigger
        {
            if (tilemap != null)
                tilemap.color = Color.white; // hiện rõ 100%

            if (tilemapCollider != null)
                tilemapCollider.enabled = true; // bật collider để đứng được
        }
    }
}
