using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupAndDraggable : MonoBehaviour
{
    public ItemBaseData itemData;
    private PlayerInventory playerInventory;

    [Header("Draggable Settings")]
    public bool isDraggable = false;
    private bool isDragged = false;
    private bool isPickedUp = false;
    private Vector3 dragOffset;
    public float dragSpeed = 5f;
    public bool hasBeenInInventory = false;

    [Header("Net Settings")]
    public bool isInNet = false;
    public float timeInNet = 5f; 
    private float netTimer;
    public bool requiresNet = false; 

    public bool IsInNet => isInNet; 
    public bool RequiresNet => requiresNet; 
    public bool IsTired { get; set; } 

    private void Start()
    {
        playerInventory = GameObject.FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found in the scene! Please ensure the PlayerInventory script is attached to a GameObject in the scene.");
        }
    }

    private void Update()
    {
        if (isInNet)
        {
            netTimer -= Time.deltaTime;
            if (netTimer <= 0f)
            {
                isInNet = false;
                Debug.Log("Item is no longer in the net.");
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !isPickedUp && playerInventory != null && hasBeenInInventory)
        {
            CheckForPlayerInRange();
        }

        if (isDraggable && isPickedUp && isDragged)
        {
            DragItem();
        }
    }

    public void SetInNet()
    {
        if (!isInNet)
        {
            isInNet = true;
            netTimer = timeInNet;
            Debug.Log("Item is now in the net for " + timeInNet + " seconds.");
        }
    }

    private void CheckForPlayerInRange()
    {
        if (playerInventory == null) return;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, playerInventory.overlapBoxSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                if (CanPickup())
                {
                    PickupItem();
                }
                break;
            }
        }
    }

    private bool CanPickup()
    {
        if (itemData == null)
        {
            Debug.LogWarning("Item data is missing, cannot pick up this item.");
            return false;
        }

        if (!hasBeenInInventory)
        {
            Debug.LogWarning("Cannot pick up item. This item has never been in the inventory.");
            return false;
        }

        if (requiresNet && !isInNet)
        {
            Debug.LogWarning("This item requires a Net to be picked up.");
            return false;
        }

        return true;
    }

    public void PickupItem()
    {
        if (playerInventory == null) return;

        bool added = playerInventory.AddItem(itemData);
        if (added)
        {
            Debug.Log("Picked up: " + itemData.itemName);
            isPickedUp = true;
            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("Inventory is full or unable to add item.");
        }
    }

    public void DropAndEnableDrag()
    {
        isPickedUp = true;
        isDraggable = true;
        hasBeenInInventory = true;
        gameObject.SetActive(true);
        transform.position = playerInventory.transform.position + new Vector3(1f, 0f, 0f);
        Debug.Log("Dropped and draggable: " + itemData.itemName);
    }

    private void OnMouseDown()
    {
        if (isDraggable && isPickedUp)
        {
            isDragged = true;
            dragOffset = transform.position - GetMouseWorldPosition();
        }
    }

    private void OnMouseUp()
    {
        if (isDragged)
        {
            isDragged = false;
        }
    }

    private void DragItem()
    {
        Vector3 newPosition = GetMouseWorldPosition() + dragOffset;
        transform.position = newPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, playerInventory != null ? playerInventory.overlapBoxSize : new Vector2(1, 1));
    }
}
