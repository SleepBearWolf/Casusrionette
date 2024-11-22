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

    [Header("Combination Settings")]
    public ItemBaseData combinationWith; 
    public ItemBaseData resultItem;     

    public void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
}
