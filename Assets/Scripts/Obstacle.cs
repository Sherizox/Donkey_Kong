using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public ParticleSystem particle;
    public float speed = 1f;

    private float OutOfBound = -27f;
    private bool isDying = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        if (particle != null)
        {
            particle.Stop();  // Optional: Stop if looping or PlayOnAwake is true
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying) return;  // prevent double triggers

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rigidbody.AddForce(collision.transform.right * speed, ForceMode2D.Impulse);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (particle != null)
            {
                particle.transform.parent = null;  // Detach so it doesn't get destroyed
                particle.Play();
                Destroy(particle.gameObject, particle.main.duration);
            }

            isDying = true;
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (this.transform.position.y <= OutOfBound)
        {
            Destroy(this.gameObject);
        }
    }
}