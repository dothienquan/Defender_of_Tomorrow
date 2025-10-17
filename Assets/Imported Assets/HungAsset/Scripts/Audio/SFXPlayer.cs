using UnityEngine;

public class SFXPlayer : MonoBehaviour {

    private SFXManager sfxManager;

    private AudioClip clipToPlay;

    private bool isPlayed = true;

    private Enemy enemyComponent;

    private void Start() {
        sfxManager = SFXManager.instance;
        enemyComponent = GetComponent<Enemy>();
    }

    public void PlaySFX(AudioClipType type) {

        isPlayed = false;

        if (!isPlayed) {
            GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
            AudioSource aSource = sfx.GetComponent<AudioSource>();

            switch (type) {
                case AudioClipType.GroundImpact: {
                        if (sfxManager.groundImpactSfx.Length > 0) {
                            clipToPlay = sfxManager.groundImpactSfx[Random.Range(0, sfxManager.groundImpactSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.WaterImpact: {
                        if (sfxManager.waterImpactSfx.Length > 0) {
                            clipToPlay = sfxManager.waterImpactSfx[Random.Range(0, sfxManager.waterImpactSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Footstep: {
                        if (sfxManager.footstepSfx.Length > 0) {
                            clipToPlay = sfxManager.footstepSfx[Random.Range(0, sfxManager.footstepSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Swim: {
                        if (sfxManager.swimmingSfx.Length > 0) {
                            clipToPlay = sfxManager.swimmingSfx[Random.Range(0, sfxManager.swimmingSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Slide: {
                        if (sfxManager.slideSfx.Length > 0) {
                            clipToPlay = sfxManager.slideSfx[Random.Range(0, sfxManager.slideSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Jump: {
                        if (sfxManager.jumpSfx.Length > 0) {
                            clipToPlay = sfxManager.jumpSfx[Random.Range(0, sfxManager.jumpSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Swing: {
                        if (sfxManager.swingSfx.Length > 0) {
                            clipToPlay = sfxManager.swingSfx[Random.Range(0, sfxManager.swingSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Hit: {
                        if (sfxManager.hitSfx.Length > 0) {
                            clipToPlay = sfxManager.hitSfx[Random.Range(0, sfxManager.hitSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.DrawBow: {
                        if (sfxManager.drawBowSfx.Length > 0) {
                            clipToPlay = sfxManager.drawBowSfx[Random.Range(0, sfxManager.drawBowSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Shoot: {
                        if (sfxManager.shootSfx.Length > 0) {
                            clipToPlay = sfxManager.shootSfx[Random.Range(0, sfxManager.shootSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.ArrowFly: {
                        if (sfxManager.arrowFlySfx.Length > 0) {
                            clipToPlay = sfxManager.arrowFlySfx[Random.Range(0, sfxManager.arrowFlySfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.Block: {
                        if (sfxManager.blockSfx.Length > 0) {
                            clipToPlay = sfxManager.blockSfx[Random.Range(0, sfxManager.blockSfx.Length)];
                        }
                        break;
                    }
                case AudioClipType.PotionPickup: {
                        if (sfxManager.potionSfx.Length > 0) {
                            clipToPlay = sfxManager.potionSfx[Random.Range(0, sfxManager.potionSfx.Length)];
                        }
                        break;
                    }
            }

            if (clipToPlay != null) {
                aSource.clip = clipToPlay;
                aSource.Play();
                isPlayed = false;

                Destroy(sfx, (clipToPlay.length + 0.05f));
            }
            else {
                isPlayed = false;
                Destroy(sfx);
            }
        }
    }

    public void PlaySFX_byString(string sfxType) {
        switch (sfxType) {
            case "Idle":
                if (enemyComponent.idleSfx.Length > 0) {
                    isPlayed = false;

                    if (!isPlayed) {
                        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
                        AudioSource aSource = sfx.GetComponent<AudioSource>();

                        clipToPlay = enemyComponent.idleSfx[Random.Range(0, enemyComponent.idleSfx.Length)];

                        if (clipToPlay != null) {
                            aSource.clip = clipToPlay;
                            aSource.Play();
                            isPlayed = false;

                            Destroy(sfx, (clipToPlay.length + 0.05f));
                        }
                        else {
                            isPlayed = false;
                            Destroy(sfx);
                        }
                    }
                }
                break;
            case "Attack":
                if (enemyComponent.attackSfx.Length > 0) {
                    isPlayed = false;

                    if (!isPlayed) {
                        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
                        AudioSource aSource = sfx.GetComponent<AudioSource>();

                        clipToPlay = enemyComponent.attackSfx[Random.Range(0, enemyComponent.attackSfx.Length)];

                        if (clipToPlay != null) {
                            aSource.clip = clipToPlay;
                            aSource.Play();
                            isPlayed = false;

                            Destroy(sfx, (clipToPlay.length + 0.05f));
                        }
                        else {
                            isPlayed = false;
                            Destroy(sfx);
                        }
                    }
                }
                break;
            case "Hurt":
                if (enemyComponent.hurtSfx.Length > 0) {
                    isPlayed = false;

                    if (!isPlayed) {
                        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
                        AudioSource aSource = sfx.GetComponent<AudioSource>();

                        clipToPlay = enemyComponent.hurtSfx[Random.Range(0, enemyComponent.hurtSfx.Length)];

                        if (clipToPlay != null) {
                            aSource.clip = clipToPlay;
                            aSource.Play();
                            isPlayed = false;

                            Destroy(sfx, (clipToPlay.length + 0.05f));
                        }
                        else {
                            isPlayed = false;
                            Destroy(sfx);
                        }
                    }
                }
                break;
            case "Run":
                if (enemyComponent.runSfx.Length > 0) {
                    isPlayed = false;

                    if (!isPlayed) {
                        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
                        AudioSource aSource = sfx.GetComponent<AudioSource>();

                        clipToPlay = enemyComponent.runSfx[Random.Range(0, enemyComponent.runSfx.Length)];

                        if (clipToPlay != null) {
                            aSource.clip = clipToPlay;
                            aSource.Play();
                            isPlayed = false;

                            Destroy(sfx, (clipToPlay.length + 0.05f));
                        }
                        else {
                            isPlayed = false;
                            Destroy(sfx);
                        }
                    }
                }
                break;
            case "Death":
                if (enemyComponent.deathSfx.Length > 0) {
                    isPlayed = false;

                    if (!isPlayed) {
                        GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
                        AudioSource aSource = sfx.GetComponent<AudioSource>();

                        clipToPlay = enemyComponent.deathSfx[Random.Range(0, enemyComponent.deathSfx.Length)];

                        if (clipToPlay != null) {
                            aSource.clip = clipToPlay;
                            aSource.Play();
                            isPlayed = false;

                            Destroy(sfx, (clipToPlay.length + 0.05f));
                        }
                        else {
                            isPlayed = false;
                            Destroy(sfx);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void PlaySound2D(AudioClip sound) {
        isPlayed = false;

        if (!isPlayed) {
            GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
            AudioSource aSource = sfx.GetComponent<AudioSource>();
            aSource.spatialBlend = 0f;

            if (sound != null) {
                aSource.clip = sound;
                aSource.Play();
                isPlayed = false;

                Destroy(sfx, (sound.length + 0.05f));
            }
            else {
                isPlayed = false;
                Destroy(sfx);
            }
        }
    }

    public void PlaySound3D(AudioClip sound) {
        isPlayed = false;

        if (!isPlayed) {
            GameObject sfx = Instantiate(Resources.Load("SFXResourcePrefab"), transform.position, Quaternion.identity) as GameObject;
            AudioSource aSource = sfx.GetComponent<AudioSource>();
            aSource.spatialBlend = 1f;

            if (sound != null) {
                aSource.clip = sound;
                aSource.Play();
                isPlayed = false;

                Destroy(sfx, (sound.length + 0.05f));
            }
            else {
                isPlayed = false;
                Destroy(sfx);
            }
        }
    }
}