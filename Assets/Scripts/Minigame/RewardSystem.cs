using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardItem
{
    public ItemBaseData itemData;
    public float spawnChance; // โอกาสในการสุ่ม
    public int cashValue; // มูลค่าเงินที่แปลงได้ (ถ้าเป็นหยก ทับทิม หรือเพชร)
}

public class RewardSystem : MonoBehaviour
{
    [Header("Reward Settings")]
    public List<RewardItem> rewardItems; // รายการไอเท็มรางวัล
    public Transform spawnPoint; // ตำแหน่งที่จะแสดงไอเท็ม
    public float spawnForce = 5f;

    private PlayerInventory playerInventory;
    private ShopManagerScript shopManager;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        shopManager = FindObjectOfType<ShopManagerScript>();

        if (playerInventory == null || shopManager == null)
        {
            Debug.LogError("PlayerInventory หรือ ShopManagerScript ไม่ถูกพบในฉาก! โปรดเพิ่มลงไป");
        }
    }

    public void SpawnReward()
    {
        RewardItem selectedItem = SelectRandomItem();
        if (selectedItem != null)
        {
            if (IsCashItem(selectedItem)) // ตรวจสอบว่าเป็นไอเท็มเงินหรือไม่
            {
                int cashAmount = selectedItem.cashValue;
                shopManager.coins += cashAmount; // เพิ่มเงินให้กับผู้เล่น
                shopManager.CoinText.text = "Coins: " + shopManager.coins.ToString();
                Debug.Log($"ได้รับเงิน {cashAmount}");
            }
            else
            {
                // สร้างไอเท็มทั่วไป
                GameObject spawnedItem = Instantiate(selectedItem.itemData.itemPrefab, spawnPoint.position, Quaternion.identity);
                Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
                }
                Debug.Log("ได้รับไอเท็ม: " + selectedItem.itemData.itemName);
            }
        }
    }

    private RewardItem SelectRandomItem()
    {
        float totalChance = 0f;
        foreach (var item in rewardItems)
        {
            totalChance += item.spawnChance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0f;

        foreach (var item in rewardItems)
        {
            cumulativeChance += item.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return item;
            }
        }
        return null;
    }

    private bool IsCashItem(RewardItem item)
    {
        return item.itemData.itemName == "เงิน" || item.itemData.itemName == "หยก" || item.itemData.itemName == "ทับทิม" || item.itemData.itemName == "เพชร";
    }
}
