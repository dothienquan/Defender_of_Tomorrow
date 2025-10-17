using UnityEngine;

public class EnemyMelee : MonoBehaviour {

    public Enemy enemyScript;
    public SFXPlayer sfxPlayer;

    private Player playerScript;

    private void Start() {
        playerScript = Player.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player" && collision.GetComponent<Player>()) {
            if (!collision.GetComponent<Player>().blocking) {
                sfxPlayer.PlaySFX(AudioClipType.Hit);
                playerScript.player_animator.SetTrigger("Hurt");
                if (playerScript.isInvincible == false)
                    playerScript.currentHealth -= enemyScript.damage;
            }
            else {
                //do something if attack is blocked
                sfxPlayer.PlaySFX(AudioClipType.Block);
                enemyScript.enemy_animator.SetTrigger("AttackBlocked");
            }
        }
    }
}
