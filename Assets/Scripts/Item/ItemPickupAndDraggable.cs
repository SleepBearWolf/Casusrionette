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
        if (Input.GetKeyDown(KeyCode.E) && !isPickedUp && playerInventory != null)
        {
            CheckForPlayerInRange();
        }

        if (isDraggable && isPickedUp && isDragged)
        {
            DragItem();
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

        if (itemData.itemType == ItemType.KeyItem)
        {
            Debug.Log("This is a key item, special conditions may be required.");
            return true;
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
            gameObject.SetActive(false);
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
