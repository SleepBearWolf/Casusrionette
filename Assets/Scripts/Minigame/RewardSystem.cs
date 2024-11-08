using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardItem
{
    public ItemBaseData itemData;
    public float spawnChance; // �͡��㹡������
    public int cashValue; // ��Ť���Թ����ŧ�� (������¡ �Ѻ��� ����ྪ�)
}

public class RewardSystem : MonoBehaviour
{
    [Header("Reward Settings")]
    public List<RewardItem> rewardItems; // ��¡��������ҧ���
    public Transform spawnPoint; // ���˹觷����ʴ������
    public float spawnForce = 5f;

    private PlayerInventory playerInventory;
    private ShopManagerScript shopManager;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        shopManager = FindObjectOfType<ShopManagerScript>();

        if (playerInventory == null || shopManager == null)
        {
            Debug.LogError("PlayerInventory ���� ShopManagerScript ���١��㹩ҡ! �ô����ŧ�");
        }
    }

    public void SpawnReward()
    {
        RewardItem selectedItem = SelectRandomItem();
        if (selectedItem != null)
        {
            if (IsCashItem(selectedItem)) // ��Ǩ�ͺ�����������Թ�������
            {
                int cashAmount = selectedItem.cashValue;
                shopManager.coins += cashAmount; // �����Թ���Ѻ������
                shopManager.CoinText.text = "Coins: " + shopManager.coins.ToString();
                Debug.Log($"���Ѻ�Թ {cashAmount}");
            }
            else
            {
                // ���ҧ����������
                GameObject spawnedItem = Instantiate(selectedItem.itemData.itemPrefab, spawnPoint.position, Quaternion.identity);
                Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
                }
                Debug.Log("���Ѻ�����: " + selectedItem.itemData.itemName);
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
        return item.itemData.itemName == "�Թ" || item.itemData.itemName == "�¡" || item.itemData.itemName == "�Ѻ���" || item.itemData.itemName == "ྪ�";
    }
}
