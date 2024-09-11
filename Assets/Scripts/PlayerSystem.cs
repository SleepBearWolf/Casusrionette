using UnityEngine;
using System.Collections;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private float flipDelay = 0.5f;

    private Rigidbody2D rb2d;
    private bool isGrounded;
    private bool facingRight = true;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public float minX = -10f, maxX = 10f;

    private bool isReturning = false;
    private bool isPaused = false;
    private float pauseTimer = 0f;

    private bool isPointAndClickMode = false;

    private GameObject heldObject = null;
    private Vector3 mouseOffset;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchMode();
        }

        
        if (!isPointAndClickMode && !isReturning && !isPaused)
        {
            MovePlayer();
            CheckIfGrounded();
            Jump();
            FlipCharacter();

            
            if (transform.position.x < minX || transform.position.x > maxX)
            {
                StartReturning(); 
            }
        }

        
        if (isReturning)
        {
            ReturnToBounds();
        }

        
        if (isPointAndClickMode && !isReturning)
        {
            HandlePointAndClickMode();
        }

        
        if (heldObject != null)
        {
            DragObject();
        }

       
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPaused = false;
                isReturning = true;  
            }
        }
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (!isReturning)
        {
            rb2d.velocity = new Vector2(moveInput * moveSpeed, rb2d.velocity.y);
        }
    }

    private void Jump()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void CheckIfGrounded()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void SwitchMode()
    {
        isPointAndClickMode = !isPointAndClickMode;

        
        if (heldObject != null)
        {
            DropObject();
        }

        Debug.Log("Switched to " + (isPointAndClickMode ? "Point and Click Mode" : "Normal Mode"));
    }

    private void HandlePointAndClickMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("PickUpObject"))
                {
                    PickUpObject(hit.collider.gameObject);
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
        mouseOffset = heldObject.transform.position - GetMouseWorldPosition2D();
        obj.GetComponent<Rigidbody2D>().isKinematic = true;

        Debug.Log("Picked up: " + obj.name);
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody2D>().isKinematic = false;
            heldObject = null;

            Debug.Log("Dropped object");
        }
    }

    private void DragObject()
    {
        if (heldObject != null)
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

    private void StartReturning()
    {
        isPaused = true;
        pauseTimer = 0f;

        StartCoroutine(DelayedFlip());
    }

    private IEnumerator DelayedFlip()
    {
        yield return new WaitForSeconds(flipDelay);

        if (transform.position.x < minX && !facingRight)
        {
            Flip();
        }
        else if (transform.position.x > maxX && facingRight)
        {
            Flip();
        }
    }

    private void ReturnToBounds()
    {
        float returnDirection = facingRight ? 1 : -1;
        rb2d.velocity = new Vector2(returnSpeed * returnDirection, rb2d.velocity.y);

        if (transform.position.x >= minX && transform.position.x <= maxX)
        {
            isReturning = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, -10, 0), new Vector3(minX, 10, 0));
        Gizmos.DrawLine(new Vector3(maxX, -10, 0), new Vector3(maxX, 10, 0));
    }
}
