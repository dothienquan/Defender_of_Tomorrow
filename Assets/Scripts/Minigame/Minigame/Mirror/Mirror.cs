using UnityEngine;
using DG.Tweening;

public class Mirror : MonoBehaviour
{
    private bool isRotating = false;
    private LaserBeam laserBeam;

    void Start()
    {
        laserBeam = FindFirstObjectByType<LaserBeam>(); 
    }

    void OnMouseDown()
    {
        if (!isRotating)
        {
            isRotating = true;

            transform.DORotate(
                new Vector3(0, 0, transform.eulerAngles.z + 90f),
                0.3f,
                RotateMode.FastBeyond360
            )
            .OnComplete(() =>
            {
                isRotating = false;
                if (laserBeam != null)
                {
                    laserBeam.CastLaser(); 
                }
            });
        }
    }
}
