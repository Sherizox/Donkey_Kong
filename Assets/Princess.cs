using UnityEngine;

public class Princess : MonoBehaviour
{
    
    public Animator animator;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.enabled = false; 
        }



    }
  
    // Update is called once per frame
    void Update()
    {
        
    }
}
