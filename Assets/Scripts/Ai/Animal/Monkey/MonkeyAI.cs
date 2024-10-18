using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    public enum MonkeyState { Patrol, Flee, Attack, Lure, Swing, JumpEvade, Return }
    private MonkeyState currentState;

    public float moveSpeed = 2f;
    public Vector2 patrolBoxSize = new Vector2(10f, 5f);  // ขนาดของกรอบ Patrol
    public Vector2 detectionBoxSize = new Vector2(14f, 7f);  // ขนาดของกรอบ Detection
    public Vector2 attackBoxSize = new Vector2(2f, 2f);  // ขนาดของกรอบ Attack
    public Vector2 fleeBoundary = new Vector2(20f, 10f);  // ขนาดของขอบเขตหนี

    public float escapeSpeed = 4f;
    public float swingForce = 8f;
    public float swingInterval = 2f;
    public float stunDuration = 1f;

    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 1f;

    public Transform player;
    public Transform attackPoint;

    private bool facingRight = true;
    private Vector2 initialPosition;
    private bool movingRight = true;
    private Rigidbody2D rb2d;
    private Animator animator;
    private PlayerInventory playerInventory;

    private bool isSwinging = false;
    private bool isExploring = false;

    private float explorationTimer = 0f;
    private float explorationDuration = 5f;

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

        StartCoroutine(SwingAtIntervals());
    }

    private void Update()
    {
        UpdateAnimation();

        switch (currentState)
        {
            case MonkeyState.Patrol:
                Explore();
                DetectPlayer();
                break;
            case MonkeyState.Flee:
                FleeFromPlayer();
                break;
            case MonkeyState.Lure:
                LurePlayer();
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
        // อัปเดต Animation ตามสถานะต่างๆ
    }

    private void Explore()
    {
        explorationTimer += Time.deltaTime;

        if (explorationTimer >= explorationDuration)
        {
            if (!isSwinging)
            {
                currentState = MonkeyState.Swing;
            }
            else
            {
                movingRight = !movingRight;
                Flip();
                currentState = MonkeyState.Patrol;
            }

            explorationTimer = 0f;
        }

        if (currentState == MonkeyState.Patrol && IsGroundInFront())
        {
            float moveDirection = movingRight ? 1f : -1f;
            rb2d.velocity = new Vector2(moveDirection * moveSpeed, rb2d.velocity.y);

            if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
            {
                Flip();
            }
        }
    }

    private bool IsGroundInFront()
    {
        Vector2 rayOrigin = groundCheck.position;
        Vector2 rayDirection = Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, groundCheckDistance, groundLayer);

        return hit.collider != null;
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (playerInventory.items.Count > 0)
        {
            if (distanceToPlayer < attackBoxSize.x / 2)
            {
                currentState = MonkeyState.Attack;
            }
            else if (distanceToPlayer < detectionBoxSize.x / 2)
            {
                currentState = MonkeyState.Lure;
            }
        }
        else
        {
            currentState = MonkeyState.Patrol;
        }
    }

    private void LurePlayer()
    {
        float moveDirection = movingRight ? 1f : -1f;
        rb2d.velocity = new Vector2(moveDirection * (moveSpeed * 0.5f), rb2d.velocity.y);

        explorationTimer += Time.deltaTime;
        if (explorationTimer >= explorationDuration)
        {
            movingRight = !movingRight;
            Flip();
            explorationTimer = 0f;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < attackBoxSize.x / 2)
        {
            currentState = MonkeyState.Attack;
        }
    }

    private void ChargeAttackPlayer()
    {
        if (attackPoint == null) return;

        Vector2 attackDirection = (player.position - transform.position).normalized;
        rb2d.velocity = attackDirection * moveSpeed;

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize, 0f, playerLayer);

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

        currentState = MonkeyState.Flee; // หลังโจมตีเสร็จ หนีออกจากแผนที่
    }

    private void SwingLikeSpiderMan()
    {
        if (!isSwinging)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce);
            isSwinging = true;
        }
    }

    private IEnumerator SwingAtIntervals()
    {
        while (true)
        {
            if (currentState == MonkeyState.Patrol || currentState == MonkeyState.Flee || currentState == MonkeyState.Lure)
            {
                SwingLikeSpiderMan();
                yield return new WaitForSeconds(swingInterval);
                isSwinging = false;
            }
            yield return null;
        }
    }

    private void JumpEvade()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce / 2);
        currentState = MonkeyState.Flee;
    }

    private void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        rb2d.velocity = fleeDirection * escapeSpeed;

        if (Mathf.Abs(transform.position.x) > fleeBoundary.x || Mathf.Abs(transform.position.y) > fleeBoundary.y)
        {
            // ถ้าลิงหนีออกไปไกลจากผู้เล่นจนถึงขอบเขตที่กำหนด (ยังคงอยู่ในเกม ไม่ถูกลบ)
            currentState = MonkeyState.Patrol;  // ให้กลับมาเดินสำรวจใหม่
        }
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

    private void OnDrawGizmos()
    {
        // แสดงระยะการเดิน, การตรวจจับ, และการโจมตีผ่าน Gizmos แบบ Box
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, patrolBoxSize);  // ระยะเดินสำรวจเป็น box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);  // ระยะตรวจจับผู้เล่นเป็น box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);  // ระยะโจมตีผู้เล่นเป็น box

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }
}
