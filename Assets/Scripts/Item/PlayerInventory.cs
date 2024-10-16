using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemBaseData> items = new List<ItemBaseData>();
    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;
    public Vector2 overlapBoxSize = new Vector2(2f, 2f);
    public KeyCode pickupKey = KeyCode.E;

    public int inventoryCapacity = 10;
    private GameObject heldItemObject = null;
    private ItemBaseData heldItemData = null;

    private void Start()
    {
        SetupInventorySlots();
    }

    private void SetupInventorySlots()
    {
        for (int i = 0; i < inventoryCapacity; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
            Button button = slot.GetComponent<Button>();
            int slotIndex = i;

            button.onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
    }


    private void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < items.Count)
        {
            heldItemData = items[slotIndex];
            CreateItemForDrag(heldItemData);

            RemoveItem(heldItemData);
        }
    }

    private void CreateItemForDrag(ItemBaseData itemData)
    {
        if (itemData != null && itemData.itemPrefab != null)
        {
            heldItemObject = Instantiate(itemData.itemPrefab, transform.position, Quaternion.identity);
            heldItemObject.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    private void Update()
    {
        if (heldItemObject != null)
        {
            DragHeldItem();
        }

        if (Input.GetKeyDown(pickupKey))
        {
            PickupItem();
        }

        if (Input.GetMouseButtonDown(0) && heldItemObject != null)
        {
            PlaceItem();
        }
        else if (Input.GetMouseButtonDown(1) && heldItemObject != null)
        {
            CancelItem();
        }
    }

    private void DragHeldItem()
    {
        Vector3 mousePosition = GetMouseWorldPosition2D();
        heldItemObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }

    private void PlaceItem()
    {
        if (heldItemObject != null)
        {
            heldItemObject.GetComponent<Rigidbody2D>().isKinematic = false;
            heldItemObject = null;
            heldItemData = null;
        }
    }

    private void CancelItem()
    {
        Destroy(heldItemObject);
        heldItemObject = null;
        heldItemData = null;
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
            UpdateInventoryUI();
            return true;
        }
        else
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }
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

    public void RemoveItem(ItemBaseData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            UpdateInventoryUI();
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

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventoryUIParent.childCount; i++)
        {
            Transform slot = inventoryUIParent.GetChild(i);
            Image slotImage = slot.GetComponentInChildren<Image>();
            if (i < items.Count)
            {
                slotImage.sprite = items[i].itemIcon;
                slotImage.enabled = true;
            }
            else
            {
                slotImage.enabled = false;
            }
        }
    }
}
