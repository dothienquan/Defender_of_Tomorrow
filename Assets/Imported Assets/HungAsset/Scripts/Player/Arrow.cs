using UnityEngine;

public class Arrow : MonoBehaviour {

    public int damage;
    public SFXPlayer sfxPlayer;

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Enemy" && collision.GetComponent<Enemy>()) {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.currentHealth > 0) {
                sfxPlayer.PlaySFX(AudioClipType.Hit);
                enemy.enemy_animator.SetTrigger("Hurt");
                enemy.attackedFromAfar = true;
                enemy.currentHealth -= damage;
                Destroy(gameObject);
            }
        }
    }
}
