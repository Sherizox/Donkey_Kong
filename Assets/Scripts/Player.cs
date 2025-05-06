using Spine;
using Spine.Unity;
using UnityEngine;

public class Players : MonoBehaviour
{
    private Rigidbody2D rb;
    private SkeletonAnimation skeletonAnimation;
    private CameraController cameraController;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 5f;

    private bool isGrounded;
    private bool isClimbing;
    private bool isHit = false;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (!GameManager.Instance.gameOver && !isHit && !isDead)
        {
            Move();
            Jump();
            Climb();
            UpdateAnimation();
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.eulerAngles = Vector3.zero;
        else if (moveInput < 0)
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Climb()
    {
        if (isClimbing)
        {
            float vInput = Input.GetAxis("Vertical");
            transform.position += new Vector3(0, vInput * climbSpeed * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            rb.isKinematic = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int layer = col.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            SetAnimation("idle_1", true);
        }
        else if (layer == LayerMask.NameToLayer("Objective"))
        {
            SetAnimation("idle_1", true);
            GameManager.Instance.LevelComplete();
        }
        else if (layer == LayerMask.NameToLayer("Obstacle") && !isHit && !isDead)
        {
            HandleHit();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    void HandleHit()
    {
        isHit = true;
        SetAnimation("hit", false);
        cameraController.ShakeCamera();

        rb.linearVelocity = new Vector2(-6f * Mathf.Sign(transform.localScale.x), 2f);
        GameManager.Instance.DecreaseHealth(1);

        if (GameManager.Instance.Health <= 0)
        {
            PlayDeathSequence();
        }
        else
        {
            Invoke(nameof(ResetHit), 0.6f);
        }
    }

    void ResetHit()
    {
        isHit = false;
    }

    void UpdateAnimation()
    {
      
        if (isDead) return; 

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

    void SetAnimation(string anim, bool loop)
    {
        if (skeletonAnimation.AnimationName != anim)
        {
            skeletonAnimation.state.SetAnimation(0, anim, loop);
        }
    }

    void PlayDeathSequence()
    {
        if (isDead) return; 
        isDead = true; 
        isHit = true;
        GameManager.Instance.LevelFailed();

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        cameraController.ShakeCamera();

        skeletonAnimation.state.ClearTracks();
        skeletonAnimation.state.SetAnimation(0, "dead", false); // Ensure "dead" plays once

        enabled = false;
    }
}
