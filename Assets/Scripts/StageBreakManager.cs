using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageBreakManager : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiPrefab; // Prefab ของ UI ที่จะเกิด
    public RectTransform uiContainer; // พื้นที่ที่จะแสดง UI
    public Vector2 minUISize = new Vector2(50, 50); // ขนาดต่ำสุดของ UI
    public Vector2 maxUISize = new Vector2(150, 150); // ขนาดสูงสุดของ UI

    [Header("Spawn Settings")]
    public int maxSpawns = 5; // จำนวนครั้งสูงสุดที่ UI จะเกิด
    public float spawnInterval = 3f; // ระยะเวลาการเกิด UI
    public float minX = -18f; // ตำแหน่ง X ต่ำสุด
    public float maxX = 25f; // ตำแหน่ง X สูงสุด
    public float minY = -5f; // ตำแหน่ง Y ต่ำสุด
    public float maxY = 5f; // ตำแหน่ง Y สูงสุด

    [Header("Tool Settings")]
    public Color hammerColor = Color.red;
    public Color ropeColor = Color.green;
    public Color screwdriverColor = Color.blue;
    public Color wrenchColor = Color.yellow;

    private int spawnCount = 0; // ตัวนับจำนวนการเกิด
    private PlayerItems playerItems; // ตัวอ้างอิงถึง PlayerItems เพื่อดึงข้อมูลเครื่องมือ

    private void Start()
    {
        playerItems = FindObjectOfType<PlayerItems>(); // ค้นหา PlayerItems ในเกม

        // ตรวจสอบว่า playerItems ถูกตั้งค่าหรือไม่
        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกพบในฉาก!");
            return; // หยุดการทำงานถ้า PlayerItems ไม่ถูกตั้งค่า
        }

        // ตรวจสอบว่า uiPrefab ถูกตั้งค่าใน Inspector หรือไม่
        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab ไม่ถูกตั้งค่าใน Inspector!");
            return; // หยุดการทำงานถ้า UI Prefab ไม่ถูกตั้งค่า
        }

        StartCoroutine(SpawnUIRoutine()); // เริ่มการสุ่มเกิด UI โดยใช้ Coroutine
    }

    // Coroutine สำหรับการสุ่มเกิด UI
    private IEnumerator SpawnUIRoutine()
    {
        Debug.Log("Coroutine started - Max Spawns: " + maxSpawns);

        while (spawnCount < maxSpawns)
        {
            SpawnRandomUI(); // เรียกใช้ฟังก์ชันการสุ่มเกิด UI
            Debug.Log("Spawn Count after spawning: " + spawnCount);

            // รอช่วงเวลา spawnInterval ก่อนเกิด UI ใหม่
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Reached max spawns, stopping the coroutine.");
    }

    private void SpawnRandomUI()
    {
        // ตรวจสอบว่า uiPrefab ถูกตั้งค่าแล้วหรือไม่
        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab ไม่ถูกตั้งค่าใน Inspector! ไม่สามารถสร้าง UI ได้");
            return;
        }

        // ตรวจสอบ playerItems ว่าไม่เป็น null ก่อนใช้งาน
        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกตั้งค่า! ไม่สามารถดึงข้อมูลเครื่องมือได้");
            return;
        }

        // สุ่มตำแหน่ง X และ Y
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        // สร้าง UI ใหม่จาก Prefab
        GameObject newUI = Instantiate(uiPrefab, uiContainer);

        // ตรวจสอบว่าองค์ประกอบ Image มีอยู่ใน Prefab
        Image uiImage = newUI.GetComponent<Image>();
        if (uiImage == null)
        {
            Debug.LogError("Prefab ของ UI ไม่มีคอมโพเนนต์ Image! ตรวจสอบ Prefab ใน Project");
            return;
        }

        // กำหนดตำแหน่งของ UI
        newUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);

        // สุ่มขนาดของ UI
        Vector2 randomSize = new Vector2(
            Random.Range(minUISize.x, maxUISize.x),
            Random.Range(minUISize.y, maxUISize.y)
        );
        newUI.GetComponent<RectTransform>().sizeDelta = randomSize;

        // สุ่มเครื่องมือจาก PlayerItems
        string randomTool = GetRandomToolFromPlayer();
        SetUIColor(newUI, randomTool);

        // กำหนดเครื่องมือที่จำเป็นต้องใช้ใน WorldToolUI
        WorldToolUI worldToolUI = newUI.GetComponent<WorldToolUI>();
        if (worldToolUI != null)
        {
            worldToolUI.requiredTool = randomTool;
        }
        else
        {
            Debug.LogError("Prefab ของ UI ไม่มีคอมโพเนนต์ WorldToolUI! ตรวจสอบ Prefab ใน Project");
        }

        spawnCount++; // เพิ่มจำนวนการเกิด
        Debug.Log("Spawned UI for tool: " + randomTool + ". Total spawns: " + spawnCount);
    }

    private string GetRandomToolFromPlayer()
    {
        // ตรวจสอบว่า PlayerItems มีการตั้งค่าเครื่องมือแล้วหรือไม่
        if (playerItems != null && playerItems.playerTools.Count > 0)
        {
            // สุ่มเครื่องมือจากรายการเครื่องมือที่ผู้เล่นมี
            int randomIndex = Random.Range(0, playerItems.playerTools.Count);
            return playerItems.playerTools[randomIndex].name; // คืนชื่อของเครื่องมือ
        }
        else
        {
            Debug.LogError("ไม่มีเครื่องมือใน PlayerItems");
            return "Hammer"; // คืนค่าเริ่มต้นหากไม่มีเครื่องมือ
        }
    }

    private void SetUIColor(GameObject uiElement, string tool)
    {
        Image uiImage = uiElement.GetComponent<Image>();

        // ตั้งค่าสีตามเครื่องมือ
        switch (tool)
        {
            case "Hammer":
                uiImage.color = hammerColor;
                break;
            case "Rope":
                uiImage.color = ropeColor;
                break;
            case "Screwdriver":
                uiImage.color = screwdriverColor;
                break;
            case "Wrench":
                uiImage.color = wrenchColor;
                break;
            default:
                uiImage.color = Color.white;
                break;
        }
    }
}
