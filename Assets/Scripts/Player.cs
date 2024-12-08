using Spine;
using Spine.Unity;
using UnityEngine;

public class Players : MonoBehaviour
{
 

    private Rigidbody2D rb;
    private KnightControl KnightControl;
    private bool isGrounded;
    private bool isClimbing;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 5f;
    public float stairsUp = 9f; 

    private SkeletonAnimation skeletonAnimation;

    private void Awake()
    {

        

        rb = GetComponent<Rigidbody2D>();
        KnightControl = GetComponent<KnightControl>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        Move();
        Jump();
        Climb();
        UpdateAnimation();
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");

        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        rb.velocity = moveVelocity;

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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //KnightControl.jump();
            //SetAnimation("jump", false);
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
            rb.velocity = Vector2.zero;
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
            FindObjectOfType<GameManager>().LevelComplete();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            GameManager.Instance.Health--;
           if(GameManager.Instance.Health <= 0)
            {
                SetAnimation("idle_1", false);
                SetAnimation("dead", true);
                 enabled = false;
                GameManager.Instance.LevelFailed();

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false; skeletonAnimation.loop = false;
        }
    }

    private void UpdateAnimation()
    {
        if (!isGrounded && rb.velocity.y > 0)
        {
            SetAnimation("jump", false);

        }
        else if (isGrounded && Mathf.Abs(rb.velocity.x) > 0)
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
}
