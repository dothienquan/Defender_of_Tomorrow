using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;   

public class ConfinerRoomSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private CinemachineCamera vcam; 
    private Collider2D currentRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("RoomBounds") || other == currentRoom) return;

        if (other is PolygonCollider2D poly)
            confiner.BoundingShape2D = poly;
        else if (other is CompositeCollider2D comp)
            confiner.BoundingShape2D = comp;

        currentRoom = other;

        SmoothRoomTransition();
    }

    private void SmoothRoomTransition()
    {
        Vector2 targetPos = currentRoom.bounds.center;

        DOTween.Kill("CameraRoomTween");

        vcam.transform.DOMove(
            new Vector3(targetPos.x, targetPos.y, vcam.transform.position.z),
            0.5f 
        )
        .SetEase(Ease.InOutSine)
        .SetId("CameraRoomTween");
    }
}
