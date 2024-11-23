using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public List<ItemBaseData> items = new List<ItemBaseData>();
    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;
    public int inventoryCapacity = 10;

    private int selectedIndex = -1;

    private void Start()
    {
        foreach (var item in items)
        {
            item.ResetUses();
        }
        UpdateInventoryUI(); 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && selectedIndex != -1)
        {
            DeselectItem();
        }
    }

    public ItemBaseData GetSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            return items[selectedIndex];
        }
        return null;
    }

    public bool HasItem(ItemBaseData item)
    {
        return items.Contains(item);
    }

    public bool AddItem(ItemBaseData item)
    {
        if (items.Count < inventoryCapacity)
        {
            if (item != null)
            {
                item.ResetUses();
                item.currentUses = 0;
                items.Add(item);
                CreateSlot(item);
                Debug.Log($"Added item: {item.itemName} with uses reset to {item.currentUses}/{item.maxUses}");
                return true;
            }
            else
            {
                Debug.LogWarning("Item data is null!");
                return false;
            }
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
            int index = items.IndexOf(item);
            items.Remove(item);
            DestroySlot(index);
            UpdateInventoryUI();
        }
    }

    private void CreateSlot(ItemBaseData item)
    {
        GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
        Button button = slot.GetComponent<Button>();
        int slotIndex = items.Count - 1;

        button.onClick.AddListener(() => OnSlotClicked(slotIndex));

        Image slotImage = slot.GetComponentInChildren<Image>();
        if (slotImage != null)
        {
            slotImage.sprite = item.itemIcon;
            slotImage.enabled = true;
        }
    }

    private void DestroySlot(int index)
    {
        if (index < inventoryUIParent.childCount)
        {
            Transform slot = inventoryUIParent.GetChild(index);
            Destroy(slot.gameObject);
        }
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventoryUIParent.childCount; i++)
        {
            Transform slot = inventoryUIParent.GetChild(i);
            Image slotImage = slot.GetComponentInChildren<Image>();
            Text usageText = slot.GetComponentInChildren<Text>();

            if (i < items.Count)
            {
                slotImage.sprite = items[i].itemIcon;
                slotImage.enabled = true;

                if (items[i].maxUses > 1)
                {
                    usageText.text = $"{items[i].currentUses}/{items[i].maxUses}";
                    usageText.enabled = true;
                }
                else
                {
                    usageText.enabled = false;
                }
            }
            else
            {
                slotImage.enabled = false;
                if (usageText != null) usageText.enabled = false;
            }
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryCapacity) return;

        if (slotIndex < items.Count)
        {
            if (selectedIndex == -1)
            {
                SelectItem(slotIndex);
            }
            else if (selectedIndex == slotIndex)
            {
                DeselectItem();
            }
            else
            {
                TryUseItem(selectedIndex, slotIndex); 
                DeselectItem();
            }
        }
    }

    private void SelectItem(int slotIndex)
    {
        selectedIndex = slotIndex;
        HighlightSlot(slotIndex, true);
        Debug.Log($"Selected item: {items[slotIndex].itemName}");
    }

    private void DeselectItem()
    {
        if (selectedIndex != -1)
        {
            HighlightSlot(selectedIndex, false);
            selectedIndex = -1;
            Debug.Log("Deselected item.");
        }
    }

    private void HighlightSlot(int slotIndex, bool isActive)
    {
        if (slotIndex < 0 || slotIndex >= inventoryUIParent.childCount) return;

        Transform slot = inventoryUIParent.GetChild(slotIndex);
        Image slotImage = slot.GetComponent<Image>();

        if (slotImage != null)
        {
            slotImage.color = isActive ? Color.green : Color.white;
        }
    }

    private void TryUseItem(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
        {
            Debug.LogWarning("Invalid item indices for usage.");
            return;
        }

        ItemBaseData itemA = items[indexA];
        ItemBaseData itemB = items[indexB];

        if (itemA.usageWith == itemB)
        {
            if (itemA.currentUses >= itemA.maxUses)
            {
                Debug.LogWarning($"{itemA.itemName} has reached its maximum uses and cannot be used anymore.");
                return; 
            }

            ItemBaseData resultItem = itemA.resultItem;

            if (resultItem != null)
            {
                Debug.Log($"Using {itemA.itemName} with {itemB.itemName} to create {resultItem.itemName}");

                itemA.currentUses++; 
                Debug.Log($"Current Uses: {itemA.currentUses}/{itemA.maxUses}");

                if (itemA.currentUses >= itemA.maxUses)
                {
                    Debug.Log($"{itemA.itemName} has reached its max uses and will be replaced.");
                    items[indexA] = resultItem; 
                }

                items.RemoveAt(indexB); 
                DestroySlot(indexB);

                UpdateInventoryUI();
            }
            else
            {
                Debug.LogWarning("Usage result item is not defined!");
            }
        }
        else
        {
            Debug.LogWarning($"{itemA.itemName} cannot be used with {itemB.itemName}!");
        }
    }
}
