using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageBreakManager : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiPrefab;
    public RectTransform uiContainer;
    public Vector2 minUISize = new Vector2(50, 50);
    public Vector2 maxUISize = new Vector2(150, 150);

    [Header("Spawn Settings")]
    public int maxSpawns = 5;
    public float spawnInterval = 3f;

    [Header("Zone Settings")]
    public float minX = -18f;
    public float maxX = 25f;

    
    public float topZoneMinY = 0f;
    public float topZoneMaxY = 5f;

    
    public float bottomZoneMinY = -5f;
    public float bottomZoneMaxY = -1f;

    [Header("Tool Settings")]
    public List<Color> toolColors = new List<Color>();
    public List<GameObject> toolObjects = new List<GameObject>();

    
    public List<bool> isTopZoneForTool = new List<bool>();

    private int spawnCount = 0;
    private PlayerItems playerItems;

    private void Start()
    {
        playerItems = FindObjectOfType<PlayerItems>();

        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกพบในฉาก!");
            return;
        }

        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab ไม่ถูกตั้งค่าใน Inspector!");
            return;
        }

        StartCoroutine(SpawnUIRoutine());
    }

    private IEnumerator SpawnUIRoutine()
    {
        while (spawnCount < maxSpawns)
        {
            SpawnRandomUI();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomUI()
    {
        if (uiPrefab == null || playerItems == null)
        {
            Debug.LogError("UI Prefab หรือ PlayerItems ไม่ถูกตั้งค่า! ไม่สามารถสร้าง UI ได้");
            return;
        }

        
        int randomIndex = Random.Range(0, toolObjects.Count);
        GameObject randomTool = toolObjects[randomIndex];

        
        float randomY;
        if (isTopZoneForTool[randomIndex])
        {
            randomY = Random.Range(topZoneMinY, topZoneMaxY);  
        }
        else
        {
            randomY = Random.Range(bottomZoneMinY, bottomZoneMaxY);  
        }

        
        float randomX = Random.Range(minX, maxX);

        
        GameObject newUI = Instantiate(uiPrefab, uiContainer);
        Image uiImage = newUI.GetComponent<Image>();

        if (uiImage == null)
        {
            Debug.LogError("Prefab ของ UI ไม่มีคอมโพเนนต์ Image! ตรวจสอบ Prefab ใน Project");
            return;
        }

        
        newUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);

        
        Vector2 randomSize = new Vector2(
            Random.Range(minUISize.x, maxUISize.x),
            Random.Range(minUISize.y, maxUISize.y)
        );
        newUI.GetComponent<RectTransform>().sizeDelta = randomSize;

        
        if (randomIndex < toolColors.Count)
        {
            uiImage.color = toolColors[randomIndex];
        }
        else
        {
            Debug.LogWarning("ไม่มีสีสำหรับ tool: " + randomTool.name + " - Set color to White");
            uiImage.color = Color.white;
        }

        
        WorldToolUI worldToolUI = newUI.GetComponent<WorldToolUI>();
        if (worldToolUI != null)
        {
            worldToolUI.requiredTool = randomTool.name;
        }

        spawnCount++;
    }
}
