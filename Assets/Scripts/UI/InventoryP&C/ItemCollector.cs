using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventorySystem inventorySystem; 
    public ItemBaseData itemData;

    private void Awake()
    {
        if (inventorySystem == null)
        {
            inventorySystem = FindObjectOfType<InventorySystem>();
            if (inventorySystem == null)
            {
                Debug.LogError("InventorySystem not found in the scene. Please ensure there is an InventorySystem in the scene.");
            }
        }
    }

    private void OnMouseDown()
    {
        if (inventorySystem != null && itemData != null)
        {
            if (inventorySystem.AddItem(itemData))
            {
                Debug.Log($"Picked up: {itemData.itemName}");
                Destroy(gameObject); 
            }
            else
            {
                Debug.LogWarning("Cannot add item to inventory. Inventory might be full.");
            }
        }
        else
        {
            if (inventorySystem == null)
            {
                Debug.LogError("InventorySystem is not assigned or found.");
            }
            if (itemData == null)
            {
                Debug.LogWarning("Item data is not assigned to this object.");
            }
        }
    }
}
