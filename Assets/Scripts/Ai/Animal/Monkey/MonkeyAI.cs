using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    public enum MonkeyState { Patrol, Flee, Attack, Lure, Swing, JumpEvade, Return }
    private MonkeyState currentState;

    public float moveSpeed = 2f;
    public float patrolRange = 5f;
    public float detectionRange = 7f;
    public float attackRange = 1.5f;
    public float escapeSpeed = 4f;
    public float swingForce = 8f;
    public float swingInterval = 2f; // ระยะเวลาการโหนเชือกเป็นระยะ
    public float stunDuration = 1f;

    public float attackRadius = 0.5f;
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

    private bool isSwinging = false; // ตรวจสอบว่ากำลังโหนอยู่หรือไม่
    private bool isExploring = false; // สถานะการสำรวจ

    private float explorationTimer = 0f; // ตั้งเวลาสำหรับการสำรวจ
    private float explorationDuration = 5f; // ระยะเวลาที่ลิงจะสำรวจก่อนเปลี่ยนพฤติกรรม

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

        playerInventory = player.GetComponent<PlayerInventory>(); // เชื่อมต่อกับ PlayerInventory

        // เริ่มการโหนเป็นระยะ
        StartCoroutine(SwingAtIntervals());
    }

    private void Update()
    {
        UpdateAnimation();

        switch (currentState)
        {
            case MonkeyState.Patrol:
                Explore(); // เพิ่มการสำรวจ
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
        // เพิ่มระบบการเล่น Animation ตาม state ของลิงได้ในฟังก์ชันนี้
    }

    private void Explore()
    {
        // ลิงจะสำรวจด้วยการเดินและโหนเชือกไปมา
        explorationTimer += Time.deltaTime;

        // สลับการเคลื่อนไหวระหว่างเดินและโหนเชือก
        if (explorationTimer >= explorationDuration)
        {
            if (!isSwinging)
            {
                currentState = MonkeyState.Swing; // เปลี่ยนไปโหนเชือกเพื่อสำรวจ
            }
            else
            {
                movingRight = !movingRight; // สลับทิศทางเมื่อเดินสำรวจ
                Flip();
                currentState = MonkeyState.Patrol; // กลับมาเดินสำรวจ
            }

            explorationTimer = 0f; // รีเซ็ตเวลาเพื่อทำให้สลับการสำรวจ
        }

        // เมื่อไม่โหนเชือก ให้ลิงเดินไปมาตามปกติ
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
        // ยิง Raycast ลงไปด้านหน้าเพื่อตรวจสอบว่ามีพื้นอยู่หรือไม่
        Vector2 rayOrigin = groundCheck.position;
        Vector2 rayDirection = Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, groundCheckDistance, groundLayer);

        // ถ้าพบพื้น (Raycast กระทบพื้น) จะ return true
        return hit.collider != null;
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // โจมตีผู้เล่นเฉพาะเมื่อผู้เล่นมีไอเท็มในช่องเก็บของ
        if (playerInventory.items.Count > 0)
        {
            if (distanceToPlayer < attackRange)
            {
                currentState = MonkeyState.Attack;
            }
            else if (distanceToPlayer < detectionRange)
            {
                currentState = MonkeyState.Lure; // ล่อลวงก่อนโจมตี
            }
        }
        else
        {
            currentState = MonkeyState.Patrol; // ไม่มีไอเท็ม ลิงจะไม่เป็นภัยและสำรวจ
        }
    }

    private void LurePlayer()
    {
        // พฤติกรรมล่อลวง: เดินเข้าหรือออกห่างจากผู้เล่น
        float moveDirection = movingRight ? 1f : -1f;
        rb2d.velocity = new Vector2(moveDirection * (moveSpeed * 0.5f), rb2d.velocity.y); // เดินช้าๆ เพื่อให้ผู้เล่นรู้สึกว่าลิงไม่มีภัย

        // สลับทิศทางแบบสุ่มเมื่อหมดเวลา
        explorationTimer += Time.deltaTime;
        if (explorationTimer >= explorationDuration)
        {
            movingRight = !movingRight;
            Flip();
            explorationTimer = 0f;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < attackRange)
        {
            currentState = MonkeyState.Attack; // เมื่อเข้าระยะโจมตี
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
                playerSystem.Stun(stunDuration); // ทำให้ผู้เล่น Stun
                playerSystem.PushBack((player.position - transform.position).normalized, 10f);

                if (playerInventory != null)
                {
                    playerInventory.ScatterItems(); // ทำให้ไอเท็มกระเด็นออกจากช่องเก็บของ
                }
            }
        }

        currentState = MonkeyState.Flee; // หลังจากโจมตีจะหนีทันที
    }

    // ระบบการโหนเชือกแบบ Spider-Man แต่ทำเป็นระยะๆ
    private void SwingLikeSpiderMan()
    {
        if (!isSwinging)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce); // โหนขึ้นไป
            isSwinging = true;
        }
    }

    // ฟังก์ชันควบคุมการโหนเป็นระยะ
    private IEnumerator SwingAtIntervals()
    {
        while (true) // ให้ทำงานตลอดเวลาในเกม
        {
            if (currentState == MonkeyState.Patrol || currentState == MonkeyState.Flee || currentState == MonkeyState.Lure)
            {
                SwingLikeSpiderMan(); // เรียกโหน
                yield return new WaitForSeconds(swingInterval); // รอเป็นระยะ (เช่น 2 วินาที)
                isSwinging = false; // หลังจากโหนเสร็จตั้งค่าให้ไม่โหน
            }
            yield return null; // รอ 1 frame ก่อนวนลูปต่อไป
        }
    }

    private void JumpEvade()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce / 2); // กระโดดเล็กน้อย
        currentState = MonkeyState.Flee;
    }

    private void FleeFromPlayer()
    {
        // ลิงหนีจากผู้เล่น
        if (IsGroundInFront())
        {
            Vector2 fleeDirection = (transform.position - player.position).normalized;
            rb2d.velocity = fleeDirection * escapeSpeed;
        }
        else
        {
            // ถ้าไม่มีพื้นให้เปลี่ยนทิศทาง
            movingRight = !movingRight;
            Flip();
        }
    }

    private void ReturnToInitialPosition()
    {
        // กลับไปที่ตำแหน่งเริ่มต้น
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
        // วาด Gizmos เพื่อดู Raycast ที่ใช้ตรวจสอบพื้นที่เดิน
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }
}
