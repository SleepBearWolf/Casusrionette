using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    public enum ChickenState { Patrol, Flee, Attack, Bait, JumpEvade, Return, Captured, Tired }
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

    public float tiredDuration = 5f;
    public float attackRadius = 0.5f;
    public float baitDelay = 1f;
    public float timeToEscapeNet = 5f;
    public float escapeCooldown = 2f;

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
    private float escapeTimer;
    private bool isEscaping = false;
    private bool isTired = false;
    private float tiredTimer;

    private static bool isNetOccupied = false;

    public ChickenState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public AnimationClip walkAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip fleeAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip capturedAnimation;
    public AnimationClip alertAnimation;

    public AudioClip attackSound;
    public AudioClip alertSound;
    private AudioSource audioSource;

    private Animator animator;

    private void Start()
    {
        currentState = ChickenState.Patrol;
        initialPosition = transform.position;
        currentPatrolCenter = initialPosition;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

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

            escapeTimer -= Time.deltaTime;
            if (escapeTimer <= 0f)
            {
                ReleaseChicken();
            }

            return;
        }

        if (currentState == ChickenState.Tired)
        {
            rb2d.velocity = Vector2.zero;
            tiredTimer -= Time.deltaTime;
            if (tiredTimer <= 0f)
            {
                currentState = ChickenState.Patrol;
                Debug.Log("Chicken is no longer tired and resumes patrolling.");
            }
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
        if (isEscaping || isNetOccupied) return;

        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y + overlapBoxOffsetY);
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, overlapBoxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Net"))
            {
                if (currentState != ChickenState.Captured)
                {
                    CaptureChicken(hit.gameObject);
                    isNetOccupied = true;
                }
            }
        }
    }

    public void SetTired(float duration)
    {
        currentState = ChickenState.Tired;
        tiredTimer = duration;
        Debug.Log("Chicken is tired for " + duration + " seconds.");
    }

    public void CaptureChicken(GameObject netObject)
    {
        currentState = ChickenState.Captured;
        rb2d.velocity = Vector2.zero;
        net = netObject;
        escapeTimer = timeToEscapeNet;
        isEscaping = false;

        transform.position = new Vector2(net.transform.position.x, net.transform.position.y - 0.5f);
    }

    public void ReleaseChicken()
    {
        currentState = ChickenState.Tired;
        tiredTimer = tiredDuration;

        if (net != null)
        {
            StartCoroutine(DestroyNetAfterDelay());
        }

        rb2d.velocity = Vector2.zero;
        currentPatrolCenter = transform.position;

        isWalking = true;
        movingRight = Random.Range(0, 2) == 0;

        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);

        isEscaping = true;
        isNetOccupied = false;
        StartCoroutine(ResetEscapeStatus());

        Debug.Log("Chicken released at position: " + transform.position + ", patrol center updated to: " + currentPatrolCenter);
    }

    private IEnumerator DestroyNetAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (net != null)
        {
            Destroy(net);
            net = null;
        }
    }

    private IEnumerator ResetEscapeStatus()
    {
        yield return new WaitForSeconds(escapeCooldown);
        isEscaping = false;
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
            PlayAlert();
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

    private void PlayAlert()
    {
        PlayAnimation(alertAnimation);
        if (alertSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(alertSound);
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

        FacePlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            StartCoroutine(JumpAttack());
        }
        else if (distanceToPlayer < detectionRange)
        {
            StartCoroutine(FlyAttack());
        }
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private IEnumerator JumpAttack()
    {
        if (currentState != ChickenState.Attack)
            yield break;

        PlayAnimation(attackAnimation);
        yield return new WaitForSeconds(0.2f);

        Vector2 targetPosition = player.position;
        Vector2 jumpDirection = (targetPosition - (Vector2)transform.position).normalized;

        float jumpHeight = 1f;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb2d.gravityScale);
        float velocityY = Mathf.Sqrt(2 * gravity * jumpHeight);
        float distance = Vector2.Distance(transform.position, targetPosition);
        float timeToReachTarget = distance / (moveSpeed * 1.5f);
        float velocityX = distance / timeToReachTarget;

        rb2d.velocity = new Vector2(jumpDirection.x * velocityX, velocityY);

        yield return new WaitForSeconds(timeToReachTarget);

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRadius, attackRadius), 0f, playerLayer);
        bool hitPlayer = false;

        foreach (Collider2D hitPlayerCollider in hitPlayers)
        {
            PlayerSystem playerSystem = hitPlayerCollider.GetComponent<PlayerSystem>();
            if (playerSystem != null)
            {
                playerSystem.Stun(stunDuration);
                playerSystem.PushBack((player.position - transform.position).normalized, pushBackForce);
                hitPlayer = true;
            }
        }

        rb2d.velocity = Vector2.zero;

        if (hitPlayer)
        {
            Debug.Log("Hit player, fleeing...");
            currentState = ChickenState.Flee;
        }
        else
        {
            Debug.Log("Missed player, returning to patrol...");
            currentState = ChickenState.Patrol;
        }
    }

    private IEnumerator FlyAttack()
    {
        if (currentState != ChickenState.Attack)
            yield break;

        PlayAnimation(attackAnimation);
        yield return new WaitForSeconds(0.2f);

        Vector2 flyDirection = (player.position - transform.position).normalized;
        rb2d.velocity = flyDirection * (moveSpeed * 1.5f);

        yield return new WaitForSeconds(0.5f);

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRadius, attackRadius), 0f, playerLayer);
        bool hitPlayer = false;

        foreach (Collider2D hitPlayerCollider in hitPlayers)
        {
            PlayerSystem playerSystem = hitPlayerCollider.GetComponent<PlayerSystem>();
            if (playerSystem != null)
            {
                playerSystem.Stun(stunDuration);
                playerSystem.PushBack((player.position - transform.position).normalized, pushBackForce);
                hitPlayer = true;
            }
        }

        rb2d.velocity = Vector2.zero;

        if (hitPlayer)
        {
            Debug.Log("Hit player, fleeing...");
            currentState = ChickenState.Flee;
        }
        else
        {
            Debug.Log("Missed player, returning to patrol...");
            currentState = ChickenState.Patrol;
        }
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