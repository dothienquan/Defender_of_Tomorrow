using UnityEditor;
using UnityEngine;

public class EditorCreateEnemy : Editor {

    [MenuItem("Tools/2D Game System/Create Enemy", false, 2)]
    public static void CreateEnemyMenu() {
        var enemyObject = new GameObject();
        Vector2 camPos = SceneView.lastActiveSceneView.camera.transform.position;
        enemyObject.transform.position = camPos;

        enemyObject.name = "[2DAPGS] Enemy";

        enemyObject.tag = "Enemy";
        enemyObject.layer = 9;

        enemyObject.AddComponent<SpriteRenderer>();
        enemyObject.AddComponent<Animator>();
        Rigidbody2D rigid = enemyObject.AddComponent<Rigidbody2D>();

        rigid.gravityScale = 0.4f;
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigid.constraints = RigidbodyConstraints2D.None;

        enemyObject.AddComponent<SFXPlayer>();
        enemyObject.AddComponent<Enemy>();
        enemyObject.AddComponent<BoxCollider2D>();

        Selection.activeObject = enemyObject;
        SceneView.lastActiveSceneView.LookAt(enemyObject.transform.position);
    }
}