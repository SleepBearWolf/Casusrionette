using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject minigamePrefab; // Prefab ของ minigame ที่จะใส่ใน Inspector
    public Transform dropPoint; // จุดที่ไอเท็มจะดรอปออกมา

    // Prefabs ของไอเท็มต่างๆ
    public GameObject moneyPrefab;
    public GameObject diamondPrefab;
    public GameObject goldPrefab;
    public GameObject gemPrefab;

    // อัตราการดรอปของไอเท็มแต่ละชนิด
    [Range(0, 100)] public float moneyDropRate = 50f;
    [Range(0, 100)] public float diamondDropRate = 20f;
    [Range(0, 100)] public float goldDropRate = 20f;
    [Range(0, 100)] public float gemDropRate = 10f;

    private bool isNearObject = false;
    private bool minigameCompleted = false;

    void Update()
    {
        // ตรวจสอบว่าผู้เล่นกดปุ่ม "E" ขณะอยู่ใกล้ Object
        if (isNearObject && Input.GetKeyDown(KeyCode.E))
        {
            StartMinigame();
        }
    }

    // ฟังก์ชันเริ่ม Minigame
    void StartMinigame()
    {
        if (minigamePrefab != null)
        {
            // สร้าง Minigame ขึ้นมา
            GameObject minigameInstance = Instantiate(minigamePrefab);
            MinigameController minigameController = minigameInstance.GetComponent<MinigameController>();

            // เริ่ม Minigame และส่ง callback เมื่อเล่นเสร็จ
            minigameController.StartMinigame(OnMinigameComplete);
        }
    }

    // ฟังก์ชัน callback เมื่อ Minigame เสร็จสิ้น
    void OnMinigameComplete(bool success)
    {
        if (success)
        {
            Debug.Log("Minigame completed, dropping item...");
            DropItem();
        }
        else
        {
            Debug.Log("Minigame failed, no item dropped.");
        }
    }

    // ฟังก์ชันสุ่มและดรอปไอเท็ม
    void DropItem()
    {
        float randomValue = Random.Range(0f, 100f);
        GameObject itemToDrop = null;

        if (randomValue <= gemDropRate)
        {
            itemToDrop = gemPrefab;
        }
        else if (randomValue <= gemDropRate + goldDropRate)
        {
            itemToDrop = goldPrefab;
        }
        else if (randomValue <= gemDropRate + goldDropRate + diamondDropRate)
        {
            itemToDrop = diamondPrefab;
        }
        else
        {
            itemToDrop = moneyPrefab;
        }

        if (itemToDrop != null)
        {
            Instantiate(itemToDrop, dropPoint.position, Quaternion.identity);
        }
    }

    // ตรวจสอบว่าผู้เล่นอยู่ใกล้ Object หรือไม่
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearObject = true;
            Debug.Log("Player is near the object.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearObject = false;
            Debug.Log("Player left the object.");
        }
    }
}



