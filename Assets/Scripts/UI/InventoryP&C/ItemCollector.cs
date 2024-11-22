using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public ItemBaseData itemData;

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
                Debug.LogWarning("Cannot add item to inventory.");
            }
        }
    }
}
