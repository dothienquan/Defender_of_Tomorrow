
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    [Header("HEALTH")]
    public int currentHealth; //How many health does our enemy have
    private int maximumHealth; //We store currentHealth value at start of the scene as our maximumHealth value
    public float healthFade = 0.5f; //Value at what our UI health bar will fade
    public Image healthBar; //Our UI health bar
    public CanvasGroup healthBarCanvasGroup; // Our UI health bars canvas group

    [Header("MOVING")]
    public float movingSpeed = 1.0f; //At what speed will AI move
    public float flipRate = 3.0f; //At what speed will sprite flip when turning to other side
    private float nextFlipTime = 0.0f;
    public LayerMask groundLayer; //Ground layer at what will AI move
    public Transform groundDetection; //Point from where we are detecting ground
    public float groundDetectDistance = 0.4f; //Distance at what we will detect ground 

    [Header("COMBAT")]
    public int damage = 1; //How much damage will AI apply to player
    public float meleeRate = 2.0f; //Rate at which attack animation will play (seconds of pause between animations)
    private float nextAttackTime = 0.0f;
    public float chaseDistance = 10.0f; //At what distance will AI start following player
    public float stopDistance = 2.0f;

    [Header("SOUND AND FX")]
    public GameObject effectsPrefab;  //Dust particles when AI fall to the ground
    private bool dustImpact = false;
    public Vector2 effectsPos;

    public AudioClip[] idleSfx;
    public AudioClip[] attackSfx;
    public AudioClip[] hurtSfx;
    public AudioClip[] runSfx;
    public AudioClip[] deathSfx;

    [HideInInspector]
    public Collider2D enemy_collider;
    [HideInInspector]
    public Animator enemy_animator;
    [HideInInspector]
    public Rigidbody2D enemy_rigidbody2d;
    [HideInInspector]
    public SFXPlayer sfxPlayer;

    private bool enemy_grounded = false;

    private Vector3 startingPosition;
    private float startingPositionDistance;

    private bool playerInSight = false;
    public bool attackedFromAfar = false;
    private Transform playerTransform;
    private Player playerScript;
    private float playerDistance;

    public Image bossHealthBar; //This is used to show UI Boss Health Bar
    public CanvasGroup bossHealthGroup;

    public void Start() {

        // At start we are gathering all components for our AI
        enemy_collider = GetComponent<BoxCollider2D>();
        enemy_animator = GetComponent<Animator>();
        enemy_rigidbody2d = GetComponent<Rigidbody2D>();
        sfxPlayer = GetComponent<SFXPlayer>();

        //We are also finding our player Game Object
        playerScript = Player.instance;
        playerTransform = playerScript.transform;

        //We are storing AIs starting position so we can return him to it if he stops following player
        startingPosition = transform.position;

        //Maximum health is our current health at start
        maximumHealth = currentHealth;

        //If boss health bar is assigned we want to fade it out
        if (bossHealthBar && bossHealthGroup) {
            bossHealthGroup = bossHealthBar.transform.parent.GetComponent<CanvasGroup>();
        }

        //If health bar is assigned we want to fade it out
        if (healthBar && healthBarCanvasGroup) {
            healthBarCanvasGroup.alpha = 0f;
        }
    }

    public void FixedUpdate() {
        // Clamping AIs rigidbody speed
        enemy_rigidbody2d.linearVelocity = Vector2.ClampMagnitude(enemy_rigidbody2d.linearVelocity, movingSpeed);
    }

    public void Update() {

        //If our AI is has zero health...
        if (currentHealth <= 0) {
            //Making sure the health does not go bellow zero
            currentHealth = 0;

            //Reseting AIs rigidbody speed to zero
            enemy_rigidbody2d.linearVelocity = Vector2.zero; 

            //Setting our animator to play Dead animation
            enemy_animator.SetBool("Dead", true);

            //Fading our health bar
            if (healthBarCanvasGroup.alpha > 0f) {
                healthBarCanvasGroup.alpha -= Time.deltaTime / (healthFade * 4f);
            }
        }
        else {
            //If player is in sight and if he has above 0 health points
            if (playerInSight && playerScript.currentHealth > 0) {

                //We must check where is players position based on our AIs position on X axis and we are going to flip enemy to that direction
                if(transform.position.x < playerTransform.position.x) {
                    if (Time.time >= nextFlipTime) {
                        transform.localEulerAngles = new Vector2(0f, 180f);
                        if (healthBar != null)
                            healthBar.transform.localEulerAngles = new Vector2(0f, 180f);
                        nextFlipTime = Time.time + 1f / flipRate;
                    }
                }
                else if(transform.position.x > playerTransform.position.x) {
                    if (Time.time >= nextFlipTime) {
                        transform.localEulerAngles = new Vector2(0f, 0f);
                        if (healthBar != null)
                            healthBar.transform.localEulerAngles = new Vector2(0f, 0f);
                        nextFlipTime = Time.time + 1f / flipRate;
                    }
                }
            }
            else {

                //If enemy is moving without seeing player we want to flip him in the direction of his movement
                if (enemy_rigidbody2d.linearVelocity.x > 0.05f) {
                    if (Time.time >= nextFlipTime) {
                        transform.localEulerAngles = new Vector2(0f, 180f);
                        if (healthBar != null)
                            healthBar.transform.localEulerAngles = new Vector2(0f, 180f);
                        nextFlipTime = Time.time + 1f / flipRate;
                    }
                }
                else if (enemy_rigidbody2d.linearVelocity.x < -0.05f) {
                    if (Time.time >= nextFlipTime) {
                        transform.localEulerAngles = new Vector2(0f, 0f);
                        if (healthBar != null)
                            healthBar.transform.localEulerAngles = new Vector2(0f, 0f);
                        nextFlipTime = Time.time + 1f / flipRate;
                    }
                }
            }

            //If AIs rigidbody is moving we want to play moving animation
            if (enemy_rigidbody2d.linearVelocity.magnitude > 0.1f) {
                enemy_animator.SetInteger("AnimState", 2);
            }
            else {
                enemy_animator.SetInteger("AnimState", 0);
            }
            
            //If players distance is lower than chaseDistance value - or if player attacked from afar - we want to set that AI sees our player
            if (playerDistance <= chaseDistance || attackedFromAfar == true) {
                playerInSight = true;
            }
            else if(playerDistance > chaseDistance && attackedFromAfar == false ) {
                playerInSight = false;
            }

            //Down bellow we will make our AI attack player if certain conditions are met
            if (playerScript.currentHealth > 0) { //If players health is above 0 ...
                if (enemy_grounded) { //... if our enemy is grounded ...
                    if (playerDistance > stopDistance) { //... if distance between player and AI is bigger than AIs stopping distance ...
                        if (playerInSight) { //... if player is in sight of our AI ...

                            //... we will cast a Raycast to check if there is ground in front of our AI so he can move towards player
                            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectDistance, groundLayer);
                            Debug.DrawRay(groundDetection.position, Vector2.down * groundDetectDistance, Color.red);

                            if (groundInfo.collider) {
                                float yDistance = Mathf.Abs((transform.position - playerTransform.position).y);

                                if (yDistance < 0.5f) {
                                    var target = new Vector2(playerTransform.position.x - transform.position.x, 0f);
                                    enemy_rigidbody2d.linearVelocity = target.normalized * movingSpeed;
                                }
                                else {
                                    enemy_rigidbody2d.linearVelocity = Vector2.zero;
                                }

                            }
                            else {
                                enemy_rigidbody2d.linearVelocity = Vector2.zero;
                            }
                        }
                        else {
                            //If player is not in sight of AI we will use our startingPosition value that we set in Start() function and we will move AI back to it
                            if (startingPositionDistance > Random.Range(0.9f, 1.5f)) {
                                var target = new Vector2(startingPosition.x - transform.position.x, 0f);
                                enemy_rigidbody2d.linearVelocity = target.normalized * movingSpeed;
                                enemy_rigidbody2d.AddForce(target.normalized * movingSpeed);
                            }
                        }
                    }
                    else { //If players distance is lower than AIs stopping distance we want our AI to attack
                        enemy_rigidbody2d.linearVelocity = Vector2.zero; //Stop AI from moving

                        if (Time.time >= nextAttackTime) { //By using our meleeRate value we will set our AIs attacking rate
                            enemy_animator.SetTrigger("Attack"); //Playing our animation for attacking
                            nextAttackTime = Time.time + 1f / meleeRate; //Reseting attack rate
                        }
                    }
                }
            }
            else {
                //If nothing of above is true we will use our startingPosition value that we set in Start() function and we will move AI back to it
                if (startingPositionDistance > stopDistance) {
                    var target = new Vector2(startingPosition.x - transform.position.x, 0f);
                    enemy_rigidbody2d.linearVelocity = target.normalized * movingSpeed;
                    enemy_rigidbody2d.AddForce(target.normalized * movingSpeed);
                }
            }

            //Here we check if our AI is grounding by checking if his collider is touching our groundLayer value
            if (enemy_collider.IsTouchingLayers(groundLayer)) {
                enemy_grounded = true;

                if (!dustImpact) { //If AI was in air we want to play dust particle in the moment when he hits the ground
                    ContactPoint2D[] contact = new ContactPoint2D[1];
                    enemy_collider.GetContacts(contact);
                    Vector2 contactPoint = contact[0].point;
                    var pos = new Vector2(contactPoint.x + effectsPos.x, contactPoint.y + effectsPos.y);

                    GameObject effectGo = Instantiate(effectsPrefab, pos, Quaternion.identity);
                    effectGo.GetComponent<Effect>().effectsAnimator.SetInteger("EffectsState", 1);
                    sfxPlayer.PlaySFX(AudioClipType.GroundImpact);
                    dustImpact = true;
                }
            }
            else {
                enemy_grounded = false;
                dustImpact = false;
            }

            enemy_animator.SetBool("Grounded", enemy_grounded);
        }

        //If healthBar value is set we want to be able to fade it in and out
        if (healthBar != null) {
            if (playerInSight == true && currentHealth > 0) {
                if (healthBarCanvasGroup.alpha < 0.95f) {
                    healthBarCanvasGroup.alpha += Time.deltaTime / healthFade;
                }
            }
            else if (!playerInSight) {
                if (healthBarCanvasGroup.alpha > 0f) {
                    healthBarCanvasGroup.alpha -= Time.deltaTime / healthFade;
                }
            }

            healthBar.fillAmount = (float)currentHealth / (float)maximumHealth;
        }

        //If bossHealthBar value is set we want to be able to fade it in and out
        if (bossHealthBar != null) {
            if (playerInSight && currentHealth > 0) {
                if (bossHealthGroup.alpha < 0.95f) {
                    bossHealthGroup.alpha += Time.deltaTime / healthFade;
                }
            }
            else {
                if (bossHealthGroup.alpha > 0f) {
                    bossHealthGroup.alpha -= Time.deltaTime / healthFade;
                }
            }

            bossHealthBar.fillAmount = (float)currentHealth / (float)maximumHealth;
        }

        //We have made possible for our AI to detect player if player has attacked it from afar, but we want to be able to reset him to default behavior if player choose not to combat AI
        //so we must set attackedFromAfat value to false after certain amount of time
        if(attackedFromAfar) {
            StartCoroutine(ResetAttackFromAfar());
        }
    }

    public void LateUpdate() {
        playerDistance = Vector3.Distance(transform.position, playerTransform.position);
        startingPositionDistance = Vector3.Distance(transform.position, startingPosition);
    }

    IEnumerator ResetAttackFromAfar() {        
        //We have made possible for our AI to detect player if player has attacked it from afar, but we want to be able to reset him to default behavior if player choose not to combat AI
        //so we must set attackedFromAfat value to false after certain amount of time
        yield return new WaitForSeconds(3.0f);
        attackedFromAfar = false;
    }
}
