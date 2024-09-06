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

    public Transform cameraTransform;   
    public float smoothSpeed = 0.125f;  
    public Vector3 cameraOffset;        

    [SerializeField] private float minX, maxX, minY, maxY; 

    private bool isPointAndClickMode = false; 

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
        else
        {
            Debug.LogError("No Rigidbody On GameObject! add Rigidbody or Rigidbody2D");
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchMode();
        }

        
        if (!isPointAndClickMode)
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
        else
        {
            
            HandlePointAndClickMode();
        }
    }

    void LateUpdate()
    {
        
        CameraFollow();
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
        if (is3DMode)
        {
            
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
           
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1; 
            transform.localScale = theScale;
        }
    }

    private void CameraFollow()
    {
        
        Vector3 desiredPosition = transform.position + cameraOffset;

        
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        
        Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, desiredPosition, smoothSpeed);
        cameraTransform.position = smoothedPosition;
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

    
    private void SwitchMode()
    {
        isPointAndClickMode = !isPointAndClickMode;  
        if (isPointAndClickMode)
        {
            Debug.Log("Point & ClickMode");
        }
        else
        {
            Debug.Log("PlatformerMode");
        }
    }

    
    private void HandlePointAndClickMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("Clicked on: " + hit.collider.name);
                
            }
        }
    }
}
