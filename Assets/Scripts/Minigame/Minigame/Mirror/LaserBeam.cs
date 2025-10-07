using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    public int maxReflections = 10;
    public float maxDistance = 100f;
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        CastLaser(); 
    }

    public void CastLaser() 
    {
        Vector2 position = transform.position;
        Vector2 direction = transform.right;

        lr.positionCount = 1;
        lr.SetPosition(0, position);

        int reflections = 0;

        while (reflections < maxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, direction, maxDistance);

            if (hit.collider)
            {
                reflections++;
                lr.positionCount++;
                lr.SetPosition(reflections, hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector2.Reflect(direction, hit.normal);
                    position = hit.point + direction * 0.01f;
                }
                else if (hit.collider.CompareTag("Goal"))
                {
                    Debug.Log("Laser reached goal!");
                    var goal = hit.collider.GetComponent<Goal>();
                    if (goal != null)
                    {
                        goal.Activate();
                    }
                    break;
                }
                else
                {
                    break; // tường
                }
            }
            else
            {
                lr.positionCount++;
                lr.SetPosition(reflections + 1, position + direction * maxDistance);
                break;
            }
        }
    }
}
