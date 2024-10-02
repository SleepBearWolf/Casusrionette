using System.Collections;
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
                StartDrop();
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
        if (items.Count == 0) return;

        isDroppingItem = true;
        ItemBaseData selectedItem = items[selectedItemIndex];

        if (selectedItem.itemType == ItemType.GeneralItem && selectedItem.itemPrefab != null)
        {
            Debug.Log("Preparing to drop item: " + selectedItem.itemName);
            GameObject droppedItem = Instantiate(selectedItem.itemPrefab, transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity);
            heldItemObject = droppedItem;
            heldItemObject.GetComponent<Rigidbody2D>().isKinematic = true;

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

    public void RemoveItem(ItemBaseData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log(item.itemName + " removed from inventory.");
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, overlapBoxSize);
    }
}
