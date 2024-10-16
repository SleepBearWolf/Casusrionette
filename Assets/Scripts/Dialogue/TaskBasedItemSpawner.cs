using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBasedItemSpawner : MonoBehaviour
{
    public List<ItemBaseData> itemsToSpawn;  // รายการไอเทมที่จะได้รับ
    public List<ItemBaseData> requiredItems; // รายการไอเทมที่ต้องใช้ในการแลกเปลี่ยน
    public Transform spawnPoint;             // ตำแหน่งที่ไอเทมจะปรากฏ
    public float spawnForce = 5f;            // ความแรงที่ใช้ในการปล่อยไอเทม
    public bool isExchangeRequired;          // ตรวจสอบว่าต้องแลกเปลี่ยนไอเทมหรือไม่

    private PlayerInventory playerInventory;
    private bool taskCompleted = false;      // ตรวจสอบว่าภารกิจเสร็จหรือยัง

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();  // หา PlayerInventory ในฉาก
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found in the scene!");
        }
    }

    // ฟังก์ชันเรียกจาก DE เมื่อผู้เล่นเลือกคำตอบในระบบสนทนา
    public void AttemptTask()
    {
        if (taskCompleted)
        {
            Debug.Log("Task already completed.");
            return;
        }

        if (isExchangeRequired)
        {
            if (HasRequiredItems())
            {
                RemoveRequiredItems();  // ลบไอเทมที่ใช้จาก Inventory
                SpawnItems();           // Spawn ไอเทมรางวัล
                taskCompleted = true;   // ภารกิจเสร็จแล้ว
            }
            else
            {
                Debug.Log("You don't have the required items to complete the task.");
            }
        }
        else
        {
            SpawnItems();  // หากไม่ต้องใช้การแลกเปลี่ยนก็ให้รางวัลเลย
            taskCompleted = true;
        }
    }

    // ฟังก์ชันตรวจสอบว่าใน Inventory มีไอเทมที่ต้องการหรือไม่
    private bool HasRequiredItems()
    {
        foreach (ItemBaseData requiredItem in requiredItems)
        {
            if (!playerInventory.HasItem(requiredItem, 1))
            {
                return false;
            }
        }
        return true;
    }

    // ฟังก์ชันลบไอเทมที่ต้องการออกจาก Inventory
    private void RemoveRequiredItems()
    {
        foreach (ItemBaseData requiredItem in requiredItems)
        {
            playerInventory.RemoveItem(requiredItem);
        }
    }

    // ฟังก์ชันสำหรับการ Spawn ไอเทม
    private void SpawnItems()
    {
        foreach (ItemBaseData item in itemsToSpawn)
        {
            GameObject spawnedItem = Instantiate(item.itemPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
            }

            Debug.Log("Spawned item: " + item.itemName);
        }
    }

    // วาด Gizmos ใน Scene View เพื่อช่วยแสดงตำแหน่ง Spawn
    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnPoint.position, new Vector3(1f, 1f, 1f));
        }
    }
}
