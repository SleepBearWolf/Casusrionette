using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    public enum MonkeyState { Patrol, Flee, Attack, Lure, Swing, JumpEvade, Return, Captured, CollectAndEat }
    public MonkeyState CurrentState { get; private set; }
    private MonkeyState currentState;

    public float moveSpeed = 2f;
    public Vector2 patrolBoxSize = new Vector2(10f, 5f);
    public Vector2 detectionBoxSize = new Vector2(14f, 7f);
    public Vector2 attackBoxSize = new Vector2(2f, 2f);
    public Vector2 fleeBoundary = new Vector2(20f, 10f);

    public float escapeSpeed = 4f;
    public float swingForce = 8f;
    public float swingInterval = 2f;
    public float stunDuration = 1f;
    public float stealingCooldown = 3f;
    public float captureDuration = 5f;
    public float explorationDuration = 5f;
    public float explorationTimer = 0f;

    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 1f;

    public Transform player;
    public Transform attackPoint;
    public Transform itemHolder;

    private bool facingRight = true;
    private Vector2 initialPosition;
    private bool movingRight = true;
    private Rigidbody2D rb2d;
    private Animator animator;
    private PlayerInventory playerInventory;

    private bool isSwinging = false;
    private bool isOnCooldown = false;
    private bool isCaptured = false;

    private float cooldownTimer = 0f;
    private float captureTimer = 0f;

    public List<ItemBaseData> itemsToSteal;
    public List<ItemBaseData> edibleItems;
    private List<GameObject> droppedItems = new List<GameObject>();

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
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
            }
        }

        if (isCaptured)
        {
            captureTimer -= Time.deltaTime;
            if (captureTimer <= 0f)
            {
                ReleaseMonkey();
            }
            return;
        }

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
            case MonkeyState.CollectAndEat:
                CollectAndEatItems();
                break;
        }
    }

    public void CaptureMonkey(GameObject net)
    {
        if (CurrentState != MonkeyState.Captured)
        {
            CurrentState = MonkeyState.Captured;
            isCaptured = true;
            captureTimer = captureDuration;
            rb2d.velocity = Vector2.zero;
            Debug.Log("Monkey captured by net!");
        }
    }

    public void ReleaseMonkey()
    {
        if (CurrentState == MonkeyState.Captured)
        {
            CurrentState = MonkeyState.Patrol;
            isCaptured = false;
            Debug.Log("Monkey released from net!");
        }
    }

    private void ChargeAttackPlayer()
    {
        if (attackPoint == null || isOnCooldown) return;

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
                    ScatterItemsFromPlayer();
                    isOnCooldown = true;
                    cooldownTimer = stealingCooldown;
                }
            }
        }

        currentState = MonkeyState.Flee;
    }

    private void StealOrEatItem()
    {
        bool itemEaten = false;

        foreach (ItemBaseData edibleItem in edibleItems)
        {
            if (playerInventory.HasItem(edibleItem))
            {
                playerInventory.RemoveItem(edibleItem);
                Debug.Log("Monkey ate: " + edibleItem.itemName);
                itemEaten = true;
                break; 
            }
        }

        if (!itemEaten)
        {
            foreach (ItemBaseData stealItem in itemsToSteal)
            {
                if (playerInventory.HasItem(stealItem))
                {
                    playerInventory.RemoveItem(stealItem);
                    CreateStolenItemObject(stealItem);
                    Debug.Log("Monkey stole: " + stealItem.itemName);
                    break;
                }
            }
        }
    }

    private void ScatterItemsFromPlayer()
    {
        List<ItemBaseData> itemsToDrop = new List<ItemBaseData>();

        foreach (var item in playerInventory.items)
        {
            if (edibleItems.Contains(item))
            {
                Debug.Log("Destroyed edible item: " + item.itemName);
            }
            else
            {
                itemsToDrop.Add(item);
            }
        }

        playerInventory.items.Clear();
        playerInventory.UpdateInventoryUI();

        foreach (var item in itemsToDrop)
        {
            if (item != null)
            {
                GameObject itemObject = Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                Rigidbody2D itemRb = itemObject.GetComponent<Rigidbody2D>();

                if (itemRb != null)
                {
                    Vector2 scatterDirection = Random.insideUnitCircle.normalized;
                    itemRb.AddForce(scatterDirection * 5f, ForceMode2D.Impulse);
                    droppedItems.Add(itemObject);
                }

                playerInventory.items.Clear();
                playerInventory.UpdateInventoryUI();
                playerInventory.items.Add(item);
            }
        }

        Debug.Log("Monkey scattered non-edible items from the player and destroyed edible items!");
    }

    private void CollectAndEatItems()
    {
        if (droppedItems.Count == 0)
        {
            currentState = MonkeyState.Patrol;
            return;
        }

        GameObject targetItem = droppedItems[0];
        Vector2 direction = (targetItem.transform.position - transform.position).normalized;
        rb2d.velocity = direction * moveSpeed;

        if (Vector2.Distance(transform.position, targetItem.transform.position) < 0.5f)
        {
            Destroy(targetItem);
            droppedItems.RemoveAt(0);
            Debug.Log("Monkey ate an item!");
        }
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

    private void UpdateAnimation()
    {

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

    private void DetectAndMoveToItems()
    {
        foreach (ItemBaseData itemData in itemsToSteal)
        {
            GameObject itemObject = FindClosestItem(itemData.itemName);
            if (itemObject != null)
            {
                MoveToItem(itemObject);
                currentState = MonkeyState.Attack;
                return;
            }
        }

        foreach (ItemBaseData edibleItem in edibleItems)
        {
            GameObject itemObject = FindClosestItem(edibleItem.itemName);
            if (itemObject != null)
            {
                MoveToItem(itemObject);
                currentState = MonkeyState.CollectAndEat;
                return;
            }
        }

        currentState = MonkeyState.Patrol;
    }

    private GameObject FindClosestItem(string itemName)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        GameObject closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject item in items)
        {
            if (item.name == itemName)
            {
                float distance = Vector2.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }
        }

        return closestItem;
    }

    private void MoveToItem(GameObject item)
    {
        Vector2 direction = (item.transform.position - transform.position).normalized;
        rb2d.velocity = direction * moveSpeed;

        if (Vector2.Distance(transform.position, item.transform.position) < 0.5f)
        {
            if (currentState == MonkeyState.Attack)
            {
                Debug.Log("Monkey stole: " + item.name);
                Destroy(item);
            }
            else if (currentState == MonkeyState.CollectAndEat)
            {
                Debug.Log("Monkey ate: " + item.name);
                Destroy(item);
            }

            currentState = MonkeyState.Flee;
        }
    }

    private void CreateStolenItemObject(ItemBaseData itemData)
    {
        if (itemData != null && itemData.itemPrefab != null)
        {
            GameObject stolenItem = Instantiate(itemData.itemPrefab, itemHolder.position, Quaternion.identity);
            stolenItem.transform.SetParent(itemHolder);
            droppedItems.Add(stolenItem);

            Debug.Log("Created stolen item: " + itemData.itemName);
        }
        else
        {
            Debug.LogWarning("Item data or prefab is missing!");
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

    private void FleeFromPlayer()
    {
        Vector2 fleeDirection = (transform.position - player.position).normalized;
        rb2d.velocity = fleeDirection * escapeSpeed;

        if (Mathf.Abs(transform.position.x) > fleeBoundary.x || Mathf.Abs(transform.position.y) > fleeBoundary.y)
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

    private void JumpEvade()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, swingForce / 2);
        currentState = MonkeyState.Flee;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, patrolBoxSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }
}