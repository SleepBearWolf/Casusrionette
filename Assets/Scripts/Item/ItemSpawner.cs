using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public KeyCode spawnWithoutExchangeKey = KeyCode.F;
    public KeyCode spawnWithExchangeKey = KeyCode.G;

    [Header("Item Settings")]
    public List<ItemBaseData> itemsToSpawn;     
    public List<ItemBaseData> requiredItems;    

    public Transform spawnPoint;               
    public float spawnForce = 5f;               
    public float cooldownTime = 2f;             
    public Vector2 overlapBoxSize = new Vector2(1f, 1f);
    public LayerMask playerLayer;               

    [Header("Drop Settings")]
    public bool unlimitedDrops = false;         
    public int maxDropCount = 1;                
    private int currentDropCount = 0;          

    private bool canSpawn = true;               
    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found in the scene! Please ensure the PlayerInventory script is attached to a GameObject in the scene.");
        }
    }

    private void Update()
    {
        bool isPlayerInZone = Physics2D.OverlapBox(spawnPoint.position, overlapBoxSize, 0f, playerLayer);

        if (isPlayerInZone && Input.GetKeyDown(spawnWithoutExchangeKey) && canSpawn && CheckDropLimit())
        {
            SpawnItem(false);
        }

        if (isPlayerInZone && Input.GetKeyDown(spawnWithExchangeKey) && canSpawn && CheckDropLimit())
        {
            SpawnItem(true);
        }
    }

    private void SpawnItem(bool requiresItem)
    {
        if (!canSpawn) return;

        ItemBaseData selectedItem = itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

        if (requiresItem)
        {
            ItemBaseData selectedRequiredItem = requiredItems[Random.Range(0, requiredItems.Count)];

            if (!playerInventory.items.Contains(selectedRequiredItem))
            {
                Debug.Log("You don't have the required item to spawn this item.");
                return;
            }

            playerInventory.RemoveItem(selectedRequiredItem); 
        }

        GameObject spawnedItem = Instantiate(selectedItem.itemPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * spawnForce, ForceMode2D.Impulse);
        }

        Debug.Log("Spawned item: " + selectedItem.itemName);

        if (!unlimitedDrops) 
        {
            currentDropCount++;
        }

        StartCoroutine(Cooldown()); 
    }

    private bool CheckDropLimit()
    {
        if (unlimitedDrops) return true;

        return currentDropCount < maxDropCount;
    }

    private IEnumerator Cooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(cooldownTime);  
        canSpawn = true;
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnPoint.position, overlapBoxSize);
        }
    }
}
