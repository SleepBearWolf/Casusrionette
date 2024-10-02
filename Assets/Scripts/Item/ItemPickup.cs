using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemBaseData itemData;
    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found in the scene!");
        }
    }

    private void Update()
    {
        if (playerInventory != null)
        {
            if (Input.GetKeyDown(playerInventory.pickupKey))
            {
                CheckForPlayerInRange();
            }
        }
    }

    private void CheckForPlayerInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, playerInventory.overlapBoxSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PickupItem();
                break;
            }
        }
    }

    private void PickupItem()
    {
        if (playerInventory != null)
        {
            bool added = playerInventory.AddItem(itemData);
            if (added)
            {
                Debug.Log("Picked up: " + itemData.itemName);
                Destroy(gameObject); 
            }
            else
            {
                Debug.Log("Inventory is full or unable to add item.");
            }
        }
    }

    public void DropItem()
    {
        if (playerInventory != null && playerInventory.items.Contains(itemData))
        {
            playerInventory.RemoveItem(itemData);
            transform.position = playerInventory.transform.position + new Vector3(1f, 0f, 0f);
            gameObject.SetActive(true); 
            Debug.Log("Dropped: " + itemData.itemName);
        }
        else
        {
            Debug.LogWarning("Cannot drop item. It's not in the inventory.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, playerInventory != null ? playerInventory.overlapBoxSize : new Vector2(1, 1));
    }
}
