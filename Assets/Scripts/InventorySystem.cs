using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<Item> inventoryItems = new List<Item>();

    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        
    }

    public void RemoveItem(Item item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            
        }
    }

    public void ShowInventory()
    {
        foreach (Item item in inventoryItems)
        {
            
        }
    }
}

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite itemIcon;
}
