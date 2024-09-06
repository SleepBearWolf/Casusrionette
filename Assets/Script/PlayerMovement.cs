using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   
    [SerializeField] private float jumpForce = 10f;  

    private Rigidbody2D rb2d;     
    private Rigidbody rb3d;       
    private bool isGrounded;

    private bool is3DMode;        
    private bool facingRight = true; 

    void Start()
    {
        
        rb2d = GetComponent<Rigidbody2D>();
        rb3d = GetComponent<Rigidbody>();

        if (rb3d != null)
        {
            is3DMode = true; 
           
            rb3d.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else if (rb2d != null)
        {
            is3DMode = false;
            
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
    }

    void Update()
    {
        if (is3DMode)
        {
            
            MovePlayer3D();
        }
        else
        {
            
            MovePlayer2D();
        }

        
        FlipCharacter();
    }

    void MovePlayer2D()
    {
        
        float moveInput = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(moveInput * moveSpeed, rb2d.velocity.y);

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    void MovePlayer3D()
    {
        
        float moveInputX = Input.GetAxis("Horizontal");

        Vector3 move = new Vector3(moveInputX * moveSpeed, rb3d.velocity.y, rb3d.velocity.z);
        rb3d.velocity = move;

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb3d.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FlipCharacter()
    {
        
        float moveInput = Input.GetAxis("Horizontal");

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; 
        transform.localScale = theScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (is3DMode && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
        if (is3DMode && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (!is3DMode && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
       
        if (!is3DMode && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
