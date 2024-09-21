using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public float minX = -18f; 
    public float maxX = 25f; 
    public float minY = -5f; 
    public float maxY = 5f; 

    [Header("Tool Settings")]
    public Color hammerColor = Color.red;
    public Color ropeColor = Color.green;
    public Color screwdriverColor = Color.blue;
    public Color wrenchColor = Color.yellow;

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
        Debug.Log("Coroutine started - Max Spawns: " + maxSpawns);

        while (spawnCount < maxSpawns)
        {
            SpawnRandomUI(); 
            Debug.Log("Spawn Count after spawning: " + spawnCount);

            
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Reached max spawns, stopping the coroutine.");
    }

    private void SpawnRandomUI()
    {
       
        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab ไม่ถูกตั้งค่าใน Inspector! ไม่สามารถสร้าง UI ได้");
            return;
        }

        
        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกตั้งค่า! ไม่สามารถดึงข้อมูลเครื่องมือได้");
            return;
        }

        
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        
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

        
        string randomTool = GetRandomToolFromPlayer();
        SetUIColor(newUI, randomTool);

        
        WorldToolUI worldToolUI = newUI.GetComponent<WorldToolUI>();
        if (worldToolUI != null)
        {
            worldToolUI.requiredTool = randomTool;
        }
        else
        {
            Debug.LogError("Prefab ของ UI ไม่มีคอมโพเนนต์ WorldToolUI! ตรวจสอบ Prefab ใน Project");
        }

        spawnCount++; 
        Debug.Log("Spawned UI for tool: " + randomTool + ". Total spawns: " + spawnCount);
    }

    private string GetRandomToolFromPlayer()
    {
        
        if (playerItems != null && playerItems.playerTools.Count > 0)
        {
            
            int randomIndex = Random.Range(0, playerItems.playerTools.Count);
            return playerItems.playerTools[randomIndex].name; 
        }
        else
        {
            Debug.LogError("ไม่มีเครื่องมือใน PlayerItems");
            return "Hammer"; 
        }
    }

    private void SetUIColor(GameObject uiElement, string tool)
    {
        Image uiImage = uiElement.GetComponent<Image>();

        
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
