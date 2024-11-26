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
    private bool isUsingItem = false;

    private void Start()
    {
        foreach (var item in items)
        {
            item.ResetUses();
        }

        UpdateInventoryUI();

        if (inventoryUIParent == null)
        {
            Debug.LogError("InventoryUIParent is not assigned in the Inspector!");
        }
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
            Debug.Log($"Removing item: {item.itemName}");
            items.Remove(item);

            selectedIndex = -1;
            isUsingItem = false;

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
        Debug.Log("Updating Inventory UI...");

        foreach (Transform child in inventoryUIParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryUIParent);

            Image slotImage = newSlot.GetComponentInChildren<Image>();
            Text usageText = newSlot.GetComponentInChildren<Text>();

            if (slotImage != null)
            {
                slotImage.sprite = items[i].itemIcon;
                slotImage.enabled = true;
            }

            if (usageText != null)
            {
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

            int slotIndex = i; 
            newSlot.GetComponent<Button>().onClick.AddListener(() => OnSlotClicked(slotIndex));
        }

        Debug.Log("Inventory UI updated successfully.");
    }

    private void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

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
        if (isUsingItem)
        {
            Debug.LogWarning("Item is already being used!");
            return;
        }

        isUsingItem = true;

        if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
        {
            Debug.LogWarning("Invalid item indices for usage.");
            return;
        }

        ItemBaseData itemA = items[indexA];
        ItemBaseData itemB = items[indexB];

        if (itemA.usageWith == itemB)
        {
            itemA.currentUses++;
            Debug.Log($"Using {itemA.itemName}: {itemA.currentUses}/{itemA.maxUses}");

            if (itemA.currentUses >= itemA.maxUses)
            {
                items[indexA] = itemA.resultItem ?? null;

                if (itemA.resultItem == null)
                {
                    RemoveItem(itemA);
                }
            }

            RemoveItem(itemB);

            selectedIndex = -1;
            isUsingItem = false;

            Debug.Log("Item used successfully. Ready for next action.");
        }
        else
        {
            Debug.LogWarning($"{itemA.itemName} cannot be used with {itemB.itemName}!");
        }

        UpdateInventoryUI();
        isUsingItem = false;
    }
}
