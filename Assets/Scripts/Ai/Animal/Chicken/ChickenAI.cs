using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    public enum ChickenState { Patrol, Flee, Attack, Bait, JumpEvade, Return, Captured }
    private ChickenState currentState;

    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    public float detectionRange = 7f;
    public float attackRange = 1.5f;
    public float escapeSpeed = 4f;
    public float jumpForce = 6f;
    public float stunDuration = 1f;
    public float pushBackForce = 5f;
    public float maxFleeDistance = 10f;

    public float attackRadius = 0.5f;
    public float baitDelay = 1f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public Transform player;
    public Transform groundCheck;
    public Transform attackPoint;
    public GameObject net;

    public Vector2 overlapBoxSize = new Vector2(1f, 1f);
    public float overlapBoxOffsetY = -0.5f;
    private bool facingRight = false;

    private Vector2 initialPosition;
    private Vector2 currentPatrolCenter;
    private bool movingRight = false;
    private bool isGrounded = true;
    private Rigidbody2D rb2d;

    private bool isBaiting = false;
    private bool isWalking = true;
    private float walkDuration = 2f;
    private float stopDuration = 1.5f;
    private float timeToChangeDirection;

    public ChickenState CurrentState
    {
        get { return currentState; }
    }

    public AnimationClip walkAnimation;  
    public AnimationClip idleAnimation;
    public AnimationClip fleeAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip capturedAnimation;

    private Animator animator;

    private void Start()
    {
        currentState = ChickenState.Patrol;
        initialPosition = transform.position;
        currentPatrolCenter = initialPosition;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if (attackPoint == null)
        {
            Debug.LogError("Attack Point is not set. Please assign it in the Inspector.");
        }

        timeToChangeDirection = Time.time + walkDuration;
    }

    private void Update()
    {
        if (currentState == ChickenState.Captured)
        {
            rb2d.velocity = Vector2.zero;
            PlayAnimation(capturedAnimation);
            return;
        }

        CheckForNet();
        UpdateAnimation();

        switch (currentState)
        {
            case ChickenState.Patrol:
                Patrol();
                DetectPlayer();
                break;
            case ChickenState.Flee:
                FleeFromPlayer();
                break;
            case ChickenState.Attack:
                ChargeAttackPlayer();
                break;
            case ChickenState.Bait:
                StartCoroutine(BaitPlayer());
                break;
            case ChickenState.JumpEvade:
                JumpEvade();
                break;
            case ChickenState.Return:
                ReturnToInitialPosition();
                break;
        }
    }

    private void PlayAnimation(AnimationClip clip)
    {
        if (clip != null)
        {
            animator.Play(clip.name);
        }
    }

    private void CheckForNet()
    {
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y + overlapBoxOffsetY);
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, overlapBoxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Net"))
            {
                if (currentState != ChickenState.Captured)
                {
                    CaptureChicken(hit.gameObject);
                }
            }
        }
    }

    public void CaptureChicken(GameObject netObject)
    {
        currentState = ChickenState.Captured;
        rb2d.velocity = Vector2.zero;
        net = netObject;

        transform.position = new Vector2(net.transform.position.x, net.transform.position.y - 0.5f);
    }

    public void ReleaseChicken()
    {
        currentState = ChickenState.Patrol;
        net = null;
    }

    private void Patrol()
    {
        if (Time.time >= timeToChangeDirection)
        {
            if (isWalking)
            {
                rb2d.velocity = Vector2.zero;
                isWalking = false;
                timeToChangeDirection = Time.time + stopDuration;
            }
            else
            {
                isWalking = true;
                movingRight = Random.Range(0, 2) == 0;
                walkDuration = Random.Range(1f, 3f);
                timeToChangeDirection = Time.time + walkDuration;
            }
        }

        if (isWalking)
        {
            MoveChicken();
        }
    }

    private void MoveChicken()
    {
        float moveDirection = movingRight ? 1f : -1f;

        if (movingRight && transform.position.x >= currentPatrolCenter.x + patrolRange)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= currentPatrolCenter.x - patrolRange)
        {
            movingRight = true;
        }

        rb2d.velocity = new Vector2(moveDirection * moveSpeed, rb2d.velocity.y);

        if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
        {
            Flip(); 
        }
    }


    private void UpdateAnimation()
    {
        if (currentState == ChickenState.Patrol)
        {
            PlayAnimation(isWalking ? walkAnimation : idleAnimation);
        }
        else if (currentState == ChickenState.Flee)
        {
            PlayAnimation(fleeAnimation);
        }
        else if (currentState == ChickenState.Attack)
        {
            PlayAnimation(attackAnimation);
        }
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);

        if (distanceToPlayer < attackRange && !isBaiting)
        {
            currentState = ChickenState.Attack;
        }
        else if (distanceToPlayer < detectionRange && verticalDistance < 1f)
        {
            if (Random.value > 0.5f)
            {
                currentState = ChickenState.Bait;
            }
            else
            {
                currentState = ChickenState.Flee;
            }
        }
        else if (distanceToPlayer < detectionRange && verticalDistance > 1f)
        {
            currentState = ChickenState.JumpEvade;
        }
    }

    private void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        rb2d.velocity = fleeDirection * escapeSpeed;

        if (Vector2.Distance(transform.position, player.position) > detectionRange || Vector2.Distance(transform.position, initialPosition) > maxFleeDistance)
        {
            currentState = ChickenState.Return;
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
            }
        }

        currentState = ChickenState.Flee;
    }

    private IEnumerator BaitPlayer()
    {
        isBaiting = true;
        rb2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(baitDelay);

        currentState = Random.value > 0.5f ? ChickenState.Flee : ChickenState.JumpEvade;
        isBaiting = false;
    }

    private void JumpEvade()
    {
        if (isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
        currentState = ChickenState.Flee;
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.1f)
        {
            rb2d.velocity = (initialPosition - (Vector2)transform.position).normalized * moveSpeed;
        }
        else
        {
            currentState = ChickenState.Patrol;
            currentPatrolCenter = initialPosition;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale;
    }


    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(currentPatrolCenter.x, transform.position.y), new Vector2(patrolRange * 2, 1f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y + overlapBoxOffsetY), overlapBoxSize);
    }
}
