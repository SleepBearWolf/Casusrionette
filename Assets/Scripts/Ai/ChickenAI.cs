using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    private Vector2 initialPosition;
    private bool movingRight = true;

    public Transform player;
    public float detectionRange = 5f;
    public float escapeSpeed = 4f;
    public float jumpForce = 5f; 
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb2d;
    private bool isGrounded;

    private bool isCaught = false;  

    private void Start()
    {
        initialPosition = transform.position;
        rb2d = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (!isCaught)
        {
            Patrol();  
            DetectPlayer(); 
        }

        CheckIfGrounded();
    }

    private void Patrol()
    {
        if (movingRight)
        {
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            if (transform.position.x >= initialPosition.x + patrolRange)
            {
                movingRight = false; 
            }
        }
        else
        {
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            if (transform.position.x <= initialPosition.x - patrolRange)
            {
                movingRight = true;
            }
        }
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            FleeFromPlayer();  
        }
    }

    private void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;

        if (isGrounded)
        {
            rb2d.velocity = fleeDirection * escapeSpeed;
        }
        else
        {
            rb2d.AddForce(new Vector2(fleeDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void Capture()
    {
        isCaught = true;
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);  
    }

    private void OnEnable()
    {
        if (player == null)
        {
            Debug.LogError("Player object is not assigned in the Inspector.");
        }

        if (rb2d == null)
        {
            rb2d = GetComponent<Rigidbody2D>();
            if (rb2d == null)
            {
                Debug.LogError("Rigidbody2D is missing from the ChickenAI object.");
            }
        }
    }

}
