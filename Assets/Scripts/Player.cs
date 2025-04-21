using Spine;
using Spine.Unity;
using UnityEngine;

public class Players : MonoBehaviour
{
    private Rigidbody2D rb;
    private KnightControl KnightControl;
    private bool isGrounded;
    private bool isClimbing;
    private bool isHit = false;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 5f;

    private SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        KnightControl = GetComponent<KnightControl>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        if (!GameManager.Instance.gameOver && !isHit)
        {
            Move();
            Jump();
            Climb();
            UpdateAnimation();
        }
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = moveVelocity;

        if (moveInput > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Climb()
    {
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");
            transform.position += new Vector3(0, verticalInput * climbSpeed * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            rb.isKinematic = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            SetAnimation("idle_1", true);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Objective"))
        {
            enabled = false;
            GameManager.Instance.LevelComplete();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (!isHit)
            {
                isHit = true;
                SetAnimation("hit", false);
                GameManager.Instance.DecreaseHealth(1);

                // Knockback
                rb.velocity = new Vector2(-4f * Mathf.Sign(transform.localScale.x), 2f);

                Invoke(nameof(HandlePostHit), 0.5f);
            }
        }


    }
    private void HandlePostHit()
    {
        if (GameManager.Instance.Health <= 0)
        {
            PlayDeathSequence();
        }
        else
        {
            isHit = false;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
            skeletonAnimation.loop = false;
        }
    }

    private void UpdateAnimation()
    {
        if (!isGrounded && rb.linearVelocity.y > 0)
        {
            SetAnimation("jump", false);
        }
        else if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0)
        {
            SetAnimation("run", true);
        }
        else if (isGrounded)
        {
            SetAnimation("idle_1", true);
        }
    }

    private void SetAnimation(string animationName, bool loop)
    {
        if (skeletonAnimation.AnimationName != animationName)
        {
            skeletonAnimation.state.SetAnimation(0, animationName, loop);
        }
    }

    private void PlayDeathSequence()
    {
        skeletonAnimation.state.SetAnimation(0, "dead", false);
        enabled = false;
        GameManager.Instance.LevelFailed();
    }
}
