using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerSystem : MonoBehaviour
{
    [Header("Movement Settings")]
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
    private Vector3 originalPosition;

    public GameObject toolUI;
    private RectTransform toolUIRectTransform;
    public bool isToolUIVisible = false;

    public float hidePositionY = -600f;
    public float showPositionY = -440f;
    public float uiMoveSpeed = 0.5f;

    private bool isAnimating = false;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        toolUIRectTransform = toolUI.GetComponent<RectTransform>();

        toolUI.SetActive(false);
        toolUIRectTransform.anchoredPosition = new Vector2(toolUIRectTransform.anchoredPosition.x, hidePositionY);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (Input.GetMouseButtonDown(1) && heldObject != null)
        {
            ResetHeldTool();
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

    private void ResetHeldTool()
    {
        if (heldObject != null)
        {
            heldObject.GetComponent<DragAndDropTool>().CancelTool();
            heldObject = null;

            Cursor.visible = true;
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

    public bool IsPointAndClickModeActive()
    {
        return isPointAndClickMode;
    }

    private void SwitchMode()
    {
        if (isAnimating) return;

        if (heldObject != null)
        {
            var dragAndDropTool = heldObject.GetComponent<DragAndDropTool>();
            if (dragAndDropTool != null)
            {
                dragAndDropTool.CancelTool();
            }
            heldObject = null;
        }

        isPointAndClickMode = !isPointAndClickMode;

        if (isPointAndClickMode)
        {
            ShowToolUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            HideToolUI();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void ShowToolUI()
    {
        if (!isToolUIVisible)
        {
            isAnimating = true;
            toolUI.SetActive(true);
            LeanTween.moveY(toolUIRectTransform, showPositionY, uiMoveSpeed).setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => isAnimating = false);
            isToolUIVisible = true;
        }
    }

    public void HideToolUI()
    {
        if (isToolUIVisible)
        {
            isAnimating = true;
            LeanTween.moveY(toolUIRectTransform, hidePositionY, uiMoveSpeed).setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    toolUI.SetActive(false);
                    isAnimating = false;
                });
            isToolUIVisible = false;
        }
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
        if (heldObject == null)
        {
            heldObject = obj;
            originalPosition = obj.transform.position;
            obj.GetComponent<Rigidbody2D>().isKinematic = true;

            Debug.Log("Picked up: " + obj.name);
        }
        else
        {
            Debug.Log("Cannot pick up multiple items at once.");
        }
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

    private void ResetItemPosition()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = originalPosition;
            heldObject.GetComponent<Rigidbody2D>().isKinematic = false;
            heldObject = null;

            Debug.Log("Item reset to original position");
        }
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
