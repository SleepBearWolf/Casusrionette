using System.Collections;
using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    public enum ChickenState { Patrol, Flee, Attack, Return }
    private ChickenState currentState;

    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    private Vector2 initialPosition;
    private Vector2 currentPatrolCenter;
    private bool movingRight = true;

    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float escapeSpeed = 4f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb2d;
    private bool isGrounded;

    public float waitBeforeNewPatrol = 2f;
    public float stunDuration = 1f;
    public float pushBackForce = 5f;
    public float maxFleeDistance = 10f;

    private void Start()
    {
        currentState = ChickenState.Patrol;
        initialPosition = transform.position;
        currentPatrolCenter = initialPosition;
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    private void Update()
    {
        CorrectRotationIfFlipped();

        CheckIfGrounded();

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
                StartCoroutine(ChargeAttackPlayer());
                break;
            case ChickenState.Return:
                ReturnToInitialPosition();
                break;
        }
    }

    private void CorrectRotationIfFlipped()
    {
        if (Mathf.Abs(transform.rotation.z) > 0.01f)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void Patrol()
    {
        if (movingRight)
        {
            if (!IsObstacleAhead())
            {
                rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
                if (transform.position.x >= currentPatrolCenter.x + patrolRange)
                {
                    StartCoroutine(PauseBeforeTurning(false));
                }
            }
            else
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            if (!IsObstacleAhead())
            {
                rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
                if (transform.position.x <= currentPatrolCenter.x - patrolRange)
                {
                    StartCoroutine(PauseBeforeTurning(true));
                }
            }
            else
            {
                movingRight = true;
                Flip();
            }
        }
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            currentState = ChickenState.Attack;
        }
        else if (distanceToPlayer < detectionRange)
        {
            currentState = ChickenState.Flee;
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

    private IEnumerator ChargeAttackPlayer()
    {
        Vector2 chargeDirection = (player.position - transform.position).normalized;
        rb2d.velocity = chargeDirection * escapeSpeed;

        yield return new WaitForSeconds(0.5f);

        PlayerSystem playerSystem = player.GetComponent<PlayerSystem>();
        if (playerSystem != null)
        {
            playerSystem.Stun(stunDuration);
            Vector2 pushDirection = (player.position - transform.position).normalized;
            playerSystem.PushBack(pushDirection, pushBackForce);
        }

        currentState = ChickenState.Return;
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.1f)
        {
            Vector2 direction = (initialPosition - (Vector2)transform.position).normalized;
            rb2d.velocity = direction * moveSpeed;
        }
        else
        {
            currentState = ChickenState.Patrol;
            currentPatrolCenter = initialPosition;
        }
    }

    private IEnumerator PauseBeforeTurning(bool turnRight)
    {
        rb2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitBeforeNewPatrol);
        movingRight = turnRight;
        Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(currentPatrolCenter.x, transform.position.y), new Vector2(patrolRange * 2, 1f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }
}
