using UnityEngine;
using System.Collections;

public class GreenDragonController : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public Animator anim;

    [Header("Stats")]
    public float maxHealth = 200f;
    public float moveSpeed = 3f;
    public float detectRange = 10f;
    public float meleeRange = 2f;
    public float meleeDamage = 25f;
    public float projectileDamage = 15f;
    public float projectileSpeed = 8f;

    [Header("Timing")]
    public float attackCooldown = 1.2f;
    public float fireChance = 0.5f;

    private Transform player;
    private Rigidbody2D rb;
    private float health;
    private bool aggro;
    private bool attacking;
    private bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        health = maxHealth;
    }

    void Update()
    {
        if (dead || !player) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (!aggro && dist <= detectRange) aggro = true;

        if (!aggro) { Idle(); return; }

        if (!attacking) MoveTowardsPlayer();

        if (!attacking && dist <= Mathf.Max(meleeRange, 4f))
            StartCoroutine(DoAttack(dist));
    }

    void Idle()
    {
        anim.SetBool("Action", false);
        anim.SetFloat("Speed", 0);
        rb.linearVelocity = Vector2.zero;
    }

    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
        anim.SetBool("Action", true);
        anim.SetFloat("Speed", moveSpeed);

        // Flip sprite depending on direction (side-scroll)
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
    }

    IEnumerator DoAttack(float dist)
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero;

        bool useFire = Random.value < fireChance && dist > meleeRange * 0.8f;

        if (useFire)
        {
            anim.SetBool("Fire", true);
            yield return new WaitForSeconds(0.2f);

            var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var proj = go.GetComponent<GreenDragonProjectile>();
            if (proj) proj.Launch(player.position, projectileSpeed, projectileDamage);

            yield return new WaitForSeconds(0.8f);
            anim.SetBool("Fire", false);
        }
        else
        {
            anim.SetBool("Attack", true);
            yield return new WaitForSeconds(0.3f);

            Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * meleeRange * 0.6f, 1f, LayerMask.GetMask("Player"));
            if (hit)
                hit.gameObject.SendMessage("TakeDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);

            yield return new WaitForSeconds(0.4f);
            anim.SetBool("Attack", false);
        }

        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }

    public void TakeDamage(float dmg)
    {
        if (dead) return;
        health -= dmg;
        if (health <= 0)
        {
            dead = true;
            anim.Play("Death");
            if (TryGetComponent(out Collider2D col))
                col.enabled = false;
            rb.linearVelocity = Vector2.zero;
            this.enabled = false;
        }
        else
        {
            aggro = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * meleeRange * 0.6f, 1f);
    }
}
