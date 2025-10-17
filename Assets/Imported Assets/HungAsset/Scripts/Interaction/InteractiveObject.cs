using UnityEngine;

public enum Interactions {
    RestoreHealth,
    DepleteHealth
}

[RequireComponent(typeof(BoxCollider2D))]
public class InteractiveObject : MonoBehaviour {

    [Tooltip("What will this interaction do to player when activated.")]
    public Interactions interaction;
    [Tooltip("The set value of this objects interaction. \n 'Restore Health' - Value of health restored. \n 'Deplete Health' - Value of health depleted \n etc.")]
    public int value;
    [Tooltip("If checked object will be destroyed on use.")]
    public bool destroyObject = false;

    private Player player;
    private SFXPlayer sfxPlayer;

    public void Start() {
        player = FindObjectOfType<Player>();

        if (GetComponent<SFXPlayer>())
            sfxPlayer = GetComponent<SFXPlayer>();
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {

            if (interaction == Interactions.RestoreHealth) {
                player.HealthAdd(value);
                if (sfxPlayer)
                    sfxPlayer.PlaySFX(AudioClipType.PotionPickup);
            }
            else if (interaction == Interactions.DepleteHealth) {
                player.HealthRemove(value);

                player.player_animator.SetTrigger("Hurt");
                if (sfxPlayer)
                    sfxPlayer.PlaySFX(AudioClipType.Hit);
            }

            if (destroyObject)
                Destroy(gameObject);
        }
    }
}
