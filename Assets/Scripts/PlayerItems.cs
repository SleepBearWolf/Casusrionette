using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Tool,
    GeneralItem,
    KeyItem
}

[System.Serializable]
public class Item
{
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public bool isUsable;

    public Item(string name, string description, ItemType type, Sprite icon, GameObject prefab, bool usable)
    {
        this.itemName = name;
        this.itemDescription = description;
        this.itemType = type;
        this.itemIcon = icon;
        this.itemPrefab = prefab;
        this.isUsable = usable;
    }

    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
}

[System.Serializable]
public class KeyItem : Item
{
    public KeyItem(string name, string description, Sprite icon, GameObject prefab)
        : base(name, description, ItemType.KeyItem, icon, prefab, true)
    { }

    public override void Use()
    {
        Debug.Log("Using Key Item: " + itemName + " to unlock something.");
    }
}

public class PlayerItems : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>();
    public List<KeyItem> keyItems = new List<KeyItem>();

    public List<GameObject> playerTools = new List<GameObject>();
    public GameObject currentTool; 

    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.R;

    public bool HasTool(string toolName)
    {
        if (currentTool != null && currentTool.name == toolName)
        {
            return true; 
        }
        return false;
    }

    public void SetCurrentTool(GameObject tool)
    {
        if (currentTool != null && tool != null)
        {
            Debug.Log("HasTool: " + currentTool.name);
            return;  
        }

        currentTool = tool;
        if (tool != null)
        {
            Debug.Log("New Tool: " + tool.name);
        }
    }

    public void AddTool(GameObject newTool)
    {
        playerTools.Add(newTool);
        Debug.Log("Add Tool: " + newTool.name);
    }

    public void RemoveCurrentTool()
    {
        if (currentTool != null)
        {
            Debug.Log("Place Tools: " + currentTool.name);
            currentTool = null; 
        }
    }

    public bool HasItem(string itemName)
    {
        foreach (var item in playerItems)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItem(Item newItem)
    {
        if (newItem.itemType == ItemType.GeneralItem)
        {
            playerItems.Add(newItem);
            Debug.Log("Collected item: " + newItem.itemName);
        }
        else if (newItem.itemType == ItemType.KeyItem)
        {
            keyItems.Add(newItem as KeyItem);
            Debug.Log("Collected key item: " + newItem.itemName);
        }
    }

    public bool HasKeyItem(string keyItemName)
    {
        foreach (var keyItem in keyItems)
        {
            if (keyItem.itemName == keyItemName)
            {
                return true;
            }
        }
        return false;
    }

    public void UseTool()
    {
        if (currentTool != null)
        {
            var itemHolder = currentTool.GetComponent<ItemHolder>();
            if (itemHolder != null && itemHolder.item.isUsable)
            {
                itemHolder.item.Use();
            }
            else
            {
                Debug.LogWarning("No usable tool equipped.");
            }
        }
        else
        {
            Debug.LogWarning("No tool equipped.");
        }
    }

    public void UseItem(string itemName)
    {
        foreach (var item in playerItems)
        {
            if (item.itemName == itemName)
            {
                item.Use();
                return;
            }
        }
        Debug.LogWarning("Item " + itemName + " not found in inventory.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            CheckAndCollectItem();
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropGeneralItems();
        }
    }

    private void CheckAndCollectItem()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 2f), 0f);
        foreach (Collider2D collider in colliders)
        {
            var itemHolder = collider.GetComponent<ItemHolder>();
            if (itemHolder != null)
            {
                var item = itemHolder.item;
                if (item != null)
                {
                    AddItem(item);
                    Destroy(collider.gameObject);
                    Debug.Log("Collected: " + item.itemName);
                    return;
                }
            }
        }
    }
    private void DropGeneralItems()
    {
        List<Item> itemsToDrop = new List<Item>();

        foreach (var item in playerItems)
        {
            if (item.itemType == ItemType.GeneralItem && item.itemPrefab != null)
            {
                itemsToDrop.Add(item);
                Instantiate(item.itemPrefab, transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity);
            }
        }

        foreach (var item in itemsToDrop)
        {
            playerItems.Remove(item);
            Debug.Log("Dropped: " + item.itemName);
        }
    }
}
