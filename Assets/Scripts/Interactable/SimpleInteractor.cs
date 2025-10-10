// Put this on the Player
using UnityEngine;

public class SimpleInteractor : MonoBehaviour
{
    public float radius = 1.2f;
    public LayerMask interactableMask;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, interactableMask);
            foreach (var h in hits)
            {
                if (h.TryGetComponent<Interactable>(out var ia))
                {
                    ia.Interact(gameObject);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(transform.position, radius);
}
