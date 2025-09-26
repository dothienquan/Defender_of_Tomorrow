using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void UpdateAnimation()
    {
        // Reset hết về false trước
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsRunningLeft", false);
        animator.SetBool("IsRunningRight", false);

        if (moveInput == 0)
        {
            animator.SetBool("IsIdle", true);
        }
        else if (moveInput < 0)
        {
            animator.SetBool("IsRunningLeft", true);
        }
        else if (moveInput > 0)
        {
            animator.SetBool("IsRunningRight", true);
        }
    }
}
