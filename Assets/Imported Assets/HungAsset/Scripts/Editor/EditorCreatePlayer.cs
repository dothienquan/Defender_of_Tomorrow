using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCreatePlayer : Editor {

    [MenuItem("Tools/2D Game System/Create Player", false, 2)]
    public static void CreatePlayerMenu() {
        GameObject playerObject = new GameObject();
        Vector2 camPos = SceneView.lastActiveSceneView.camera.transform.position;
        playerObject.transform.position = camPos;

        playerObject.name = "[2DAPGS] Player";

        playerObject.tag = "Player";
        playerObject.layer = 10;

        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<Animator>();
        Rigidbody2D rigid = playerObject.AddComponent<Rigidbody2D>();

        rigid.gravityScale = 0.4f;
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigid.constraints = RigidbodyConstraints2D.None;

        PlayerInput playerInput = playerObject.AddComponent<PlayerInput>();
        playerInput.actions = Resources.Load("Input/PlayerInputActions") as InputActionAsset;

        playerObject.AddComponent<AudioListener>();
        playerObject.AddComponent<SFXPlayer>();
        playerObject.AddComponent<Player>();
        playerObject.AddComponent<CapsuleCollider2D>();
        playerObject.AddComponent<BoxCollider2D>();

        Selection.activeObject = playerObject;
        SceneView.lastActiveSceneView.LookAt(playerObject.transform.position);
    }
}