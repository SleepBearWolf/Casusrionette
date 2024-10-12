﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemBaseData> items = new List<ItemBaseData>();
    public Vector2 overlapBoxSize = new Vector2(2f, 2f);
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.R;

    public int inventoryCapacity = 10;

    private bool isPointAndClickMode = false;
    private bool isDroppingItem = false;
    private GameObject heldItemObject = null;
    private int selectedItemIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TogglePointAndClickMode();
        }

        if (isPointAndClickMode)
        {
            HandleItemDrop();
            ScrollToSelectItemWithKeys();
        }

        if (heldItemObject != null)
        {
            DragHeldItem();
        }

        if (Input.GetKeyDown(pickupKey))
        {
            PickupItem();
        }
    }

    private void TogglePointAndClickMode()
    {
        if (isDroppingItem)
        {
            CancelDrop();
        }

        isPointAndClickMode = !isPointAndClickMode;
        Debug.Log("Switched to " + (isPointAndClickMode ? "Point and Click Mode" : "Normal Mode"));
    }

    private void HandleItemDrop()
    {
        if (Input.GetKeyDown(dropKey))
        {
            if (isDroppingItem)
            {
                CancelDrop();
            }
            else
            {
                if (items.Count > 0)
                {
                    StartDrop();
                }
                else
                {
                    Debug.LogWarning("Cannot drop. No items in the inventory.");
                }
            }
        }
    }

    private void ScrollToSelectItemWithKeys()
    {
        if (items.Count == 0 || !isDroppingItem) return;

        bool itemChanged = false;

        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedItemIndex = (selectedItemIndex + 1) % items.Count;
            itemChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            selectedItemIndex = (selectedItemIndex - 1 + items.Count) % items.Count;
            itemChanged = true;
        }

        if (selectedItemIndex < 0 || selectedItemIndex >= items.Count)
        {
            selectedItemIndex = 0;
        }

        if (itemChanged)
        {
            if (isDroppingItem && heldItemObject != null)
            {
                Destroy(heldItemObject);
                StartDrop();
            }

            Debug.Log("Selected item: " + items[selectedItemIndex].itemName);
        }
    }

    private void StartDrop()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("No items to drop. Inventory is empty.");
            return;
        }

        isDroppingItem = true;
        ItemBaseData selectedItem = items[selectedItemIndex];

        if (selectedItem.itemType == ItemType.GeneralItem && selectedItem.itemPrefab != null)
        {
            Debug.Log("Preparing to drop item: " + selectedItem.itemName);
            GameObject droppedItem = Instantiate(selectedItem.itemPrefab, transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity);
            heldItemObject = droppedItem;
            heldItemObject.GetComponent<Rigidbody2D>().isKinematic = true;

            ItemPickupAndDraggable draggable = heldItemObject.GetComponent<ItemPickupAndDraggable>();
            if (draggable != null)
            {
                draggable.DropAndEnableDrag();
            }

            isDroppingItem = false;
        }
        else
        {
            Debug.LogWarning("Selected item cannot be dropped.");
        }
    }

    private void CancelDrop()
    {
        if (heldItemObject != null)
        {
            Destroy(heldItemObject);
            heldItemObject = null;
        }
        isDroppingItem = false;
        Debug.Log("Cancelled item drop.");
    }

    private void DragHeldItem()
    {
        if (heldItemObject != null)
        {
            Vector3 mousePosition = GetMouseWorldPosition2D();
            heldItemObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceItem();
            }
        }
    }

    private void PlaceItem()
    {
        if (heldItemObject != null)
        {
            Rigidbody2D rb = heldItemObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector2.zero;
            }

            heldItemObject = null;
            isDroppingItem = false;

            RemoveItem(items[selectedItemIndex]);
            Debug.Log("Item placed successfully.");
        }
    }

    private Vector3 GetMouseWorldPosition2D()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public bool AddItem(ItemBaseData item)
    {
        if (items.Count < inventoryCapacity)
        {
            items.Add(item);
            Debug.Log("Added item: " + item.itemName);
            return true;
        }
        else
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }
    }

    public void RemoveItem(ItemBaseData item, int amount = 1)
    {
        int itemsRemoved = 0;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] == item)
            {
                items.RemoveAt(i);
                itemsRemoved++;
            }

            if (itemsRemoved >= amount)
            {
                Debug.Log(item.itemName + " removed from inventory.");
                return;
            }
        }

        Debug.LogWarning("Not enough items to remove.");
    }

    public bool HasItem(ItemBaseData item, int amount = 1)
    {
        int itemCount = 0;

        foreach (ItemBaseData inventoryItem in items)
        {
            if (inventoryItem == item)
            {
                itemCount++;
            }
            if (itemCount >= amount)
            {
                return true;
            }
        }

        return false;
    }

    private void PickupItem()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, overlapBoxSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            ItemPickupAndDraggable itemPickup = collider.GetComponent<ItemPickupAndDraggable>();
            if (itemPickup != null)
            {
                itemPickup.PickupItem();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, overlapBoxSize);
    }
}
