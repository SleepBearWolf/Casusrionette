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

    [SerializeField] private Transform groundCheck;  
    [SerializeField] private LayerMask groundLayer;  

    private bool isPointAndClickMode = false; 

    private GameObject heldObject = null; 
    private Vector3 mouseOffset;         
    private float mouseZCoord;           

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
            Debug.LogError("no Rigidbody! add Rigidbody or Rigidbody2D");
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

        
        if (heldObject != null)
        {
            DragObject();
        }
    }

    
    private void MovePlayer2D()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(moveInput * moveSpeed, rb2d.velocity.y);

        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    
    private void MovePlayer3D()
    {
        float moveInputX = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(moveInputX * moveSpeed, rb3d.velocity.y, rb3d.velocity.z);
        rb3d.velocity = move;

        
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        
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

    
    private void SwitchMode()
    {
        isPointAndClickMode = !isPointAndClickMode;
        Debug.Log(isPointAndClickMode ? "Switched to Point & Click Mode" : "Switched to Platformer Mode");
    }

    
    private void HandlePointAndClickMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null) 
            {
                if (is3DMode)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("PickUpObject"))
                    {
                        PickUpObject(hit.collider.gameObject);
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null && hit.collider.CompareTag("PickUpObject"))
                    {
                        PickUpObject(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                DropObject(); 
            }
        }
    }

    
    private void PickUpObject(GameObject obj)
    {
        heldObject = obj; 

        if (is3DMode)
        {
            mouseZCoord = Camera.main.WorldToScreenPoint(heldObject.transform.position).z;
            mouseOffset = heldObject.transform.position - GetMouseWorldPosition3D();
            obj.GetComponent<Rigidbody>().isKinematic = true; 
        }
        else
        {
            mouseOffset = heldObject.transform.position - GetMouseWorldPosition2D();
            obj.GetComponent<Rigidbody2D>().isKinematic = true; 
        }

        Debug.Log("PickUp: " + obj.name);
    }

    
    private void DropObject()
    {
        if (is3DMode)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; 
        }
        else
        {
            heldObject.GetComponent<Rigidbody2D>().isKinematic = false; 
        }

        heldObject = null; 
        Debug.Log("Place");
    }

    
    private void DragObject()
    {
        if (is3DMode)
        {
            heldObject.transform.position = GetMouseWorldPosition3D() + mouseOffset;
        }
        else
        {
            heldObject.transform.position = GetMouseWorldPosition2D() + mouseOffset;
        }
    }

    
    private Vector3 GetMouseWorldPosition2D()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

   
    private Vector3 GetMouseWorldPosition3D()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
