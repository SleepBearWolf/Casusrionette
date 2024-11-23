using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    GeneralItem,
    KeyItem
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemBaseData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
    public Sprite itemIcon;
    public GameObject itemPrefab;

    [Header("Usage Settings")]
    public ItemBaseData usageWith;
    public ItemBaseData resultItem; 
    public int maxUses = 1; 
    public int currentUses = 1;

    public bool IsUsable()
    {
        return currentUses > 0;
    }

    public void Use()
    {
        if (currentUses > 0)
        {
            currentUses--;
        }
    }
    public void ResetUses()
    {
        currentUses = 0;
        Debug.Log($"Item {itemName} uses reset to {currentUses}/{maxUses}");
    }

}
