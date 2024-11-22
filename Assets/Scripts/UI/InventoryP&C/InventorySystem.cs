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
        UpdateInventoryUI(); 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && selectedIndex != -1) 
        {
            DeselectItem();
        }
    }

    public bool AddItem(ItemBaseData item)
    {
        if (items.Count < inventoryCapacity)
        {
            items.Add(item);
            CreateSlot(item); 
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
                TryCombineItems(selectedIndex, slotIndex);
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

    private void TryCombineItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
        {
            Debug.LogWarning("Invalid item indices for combination.");
            return;
        }

        ItemBaseData itemA = items[indexA];
        ItemBaseData itemB = items[indexB];

        if (itemA.combinationWith == itemB)
        {
            ItemBaseData resultItem = itemA.resultItem;

            if (resultItem != null)
            {
                Debug.Log($"Combining {itemA.itemName} with {itemB.itemName} to create {resultItem.itemName}");

                items[indexA] = resultItem; 
                items.RemoveAt(indexB); 

                DestroySlot(indexB); 
                UpdateInventoryUI();
            }
            else
            {
                Debug.LogWarning("Combination result item is not defined!");
            }
        }
        else
        {
            Debug.LogWarning($"{itemA.itemName} cannot be combined with {itemB.itemName}!");
        }
    }
}
