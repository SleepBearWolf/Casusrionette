using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 3f;
    public float jumpForce = 12f;
    private float moveSpeed;
    private bool isRunning = false;
    private bool isCrouching = false;
    private bool isGrounded = false;

    // HP/Actions
    public int maxHP = 100;
    private int currentHP;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Interaction Variables
    public Transform interactionPoint;
    public float interactionRadius = 0.5f;
    public LayerMask interactableLayer;

    // Ground Check
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Check if player is running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            moveSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            moveSpeed = walkSpeed;
        }

        // Check if player is crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            moveSpeed = crouchSpeed;
        }
        else
        {
            isCrouching = false;
        }

        // Move the player
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        // Interact with objects
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        // Flip character sprite based on direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Animator updates
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }
    private void Interact()
    {
        // Check for objects within interaction radius
        Collider2D[] interactables = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius, interactableLayer);
        foreach (Collider2D item in interactables)
        {
            // Call the interactable object's interaction method
            item.GetComponent<IInteractable>()?.Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
}

