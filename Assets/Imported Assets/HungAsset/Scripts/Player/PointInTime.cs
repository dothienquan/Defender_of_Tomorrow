using UnityEngine;

public class PointInTime {

    public Vector2 position;
    public Quaternion rotation;
    public Sprite sprite;
    public int health;

    public PointInTime(Vector2 _position, Quaternion _rotation, Sprite _sprite, int _health) {
        position = _position;
        rotation = _rotation;
        sprite = _sprite;
        health = _health;
    }
}
