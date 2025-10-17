using UnityEngine;

public enum AudioClipType { GroundImpact = 0, WaterImpact = 1, Hit = 2, Swing = 3, Jump = 4, Slide = 5, Footstep = 6, Shoot = 7, Swim = 8, DrawBow = 9, ArrowFly = 10, Block = 11, PotionPickup = 12, Slimey = 13}

public class SFXManager : MonoBehaviour {

    public AudioClip[] groundImpactSfx;
    public AudioClip[] waterImpactSfx;
    public AudioClip[] hitSfx;
    public AudioClip[] swingSfx;
    public AudioClip[] jumpSfx;
    public AudioClip[] slideSfx;
    public AudioClip[] footstepSfx;
    public AudioClip[] swimmingSfx;
    public AudioClip[] shootSfx;
    public AudioClip[] drawBowSfx;
    public AudioClip[] arrowFlySfx;
    public AudioClip[] blockSfx;
    public AudioClip[] potionSfx;

    public static SFXManager instance;

    private void Awake() {
        instance = this;
    }
}