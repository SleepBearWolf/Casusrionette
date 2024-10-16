using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    public enum MonkeyState { Patrol, Flee, Attack, Swing, JumpEvade, Return }
    private MonkeyState currentState;

    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    public float detectionRange = 7f;
    public float attackRange = 1.5f;
    public float escapeSpeed = 4f;
    public float jumpForce = 6f;
    public float swingForce = 8f; 
    public float stunDuration = 1f;
    public float pushBackForce = 5f;

    public float attackRadius = 0.5f;
    public LayerMask playerLayer;
    public Transform player;
    public Transform groundCheck;
    public Transform attackPoint;

    private bool facingRight = true;
    private Vector2 initialPosition;
    private bool movingRight = true;
    private bool isGrounded = true;
    private Rigidbody2D rb2d;

    private Animator animator;
    private PlayerInventory playerInventory;

    private void Start()
    {
        currentState = MonkeyState.Patrol;
        initialPosition = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        playerInventory = player.GetComponent<PlayerInventory>(); 
    }

    private void Update()
    {
        UpdateAnimation();

        switch (currentState)
        {
            case MonkeyState.Patrol:
                Patrol();
                DetectPlayer();
                break;
            case MonkeyState.Flee:
                FleeFromPlayer();
                break;
            case MonkeyState.Attack:
                ChargeAttackPlayer();
                break;
            case MonkeyState.Swing:
                SwingLikeSpiderMan();
                break;
            case MonkeyState.JumpEvade:
                JumpEvade();
                break;
            case MonkeyState.Return:
                ReturnToInitialPosition();
                break;
        }
    }

    private void UpdateAnimation()
    {

    }

    private void Patrol()
    {
        float moveDirection = movingRight ? 1f : -1f;
        if (movingRight && transform.position.x >= initialPosition.x + patrolRange)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= initialPosition.x - patrolRange)
        {
            movingRight = true;
        }
        rb2d.velocity = new Vector2(moveDirection * moveSpeed, rb2d.velocity.y);

        if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
        {
            Flip();
        }
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            currentState = MonkeyState.Attack;
        }
        else if (distanceToPlayer < detectionRange)
        {
            currentState = MonkeyState.Swing;
        }
    }

    private void ChargeAttackPlayer()
    {
        if (attackPoint == null) return;

        Vector2 attackDirection = (player.position - transform.position).normalized;
        rb2d.velocity = attackDirection * moveSpeed;

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRadius, attackRadius), 0f, playerLayer);

        foreach (Collider2D hitPlayer in hitPlayers)
        {
            PlayerSystem playerSystem = hitPlayer.GetComponent<PlayerSystem>();
            if (playerSystem != null)
            {
                playerSystem.Stun(stunDuration); 
                playerSystem.PushBack((player.position - transform.position).normalized, 10f);

                if (playerInventory != null)
                {
                    playerInventory.ScatterItems(); 
                }
            }
        }

        currentState = MonkeyState.Flee;
    }

    private void SwingLikeSpiderMan()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce); 
        currentState = MonkeyState.Flee; 
    }

    private void JumpEvade()
    {
        if (isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
        currentState = MonkeyState.Flee;
    }

    private void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        rb2d.velocity = fleeDirection * escapeSpeed;
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.1f)
        {
            rb2d.velocity = (initialPosition - (Vector2)transform.position).normalized * moveSpeed;
        }
        else
        {
            currentState = MonkeyState.Patrol;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
