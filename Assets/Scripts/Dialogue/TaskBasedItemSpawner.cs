using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBasedItemSpawner : MonoBehaviour
{
    public List<ItemBaseData> itemsToSpawn;  
    public List<ItemBaseData> requiredItems; 
    public Transform spawnPoint;            
    public float spawnForce = 5f;           
    public bool isExchangeRequired;          

    private PlayerInventory playerInventory;
    private bool taskCompleted = false;      

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();  
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found in the scene!");
        }
    }

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
                RemoveRequiredItems();  
                SpawnItems();           
                taskCompleted = true;  
            }
            else
            {
                Debug.Log("You don't have the required items to complete the task.");
            }
        }
        else
        {
            SpawnItems(); 
            taskCompleted = true;
        }
    }

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

    private void RemoveRequiredItems()
    {
        foreach (ItemBaseData requiredItem in requiredItems)
        {
            playerInventory.RemoveItem(requiredItem);
        }
    }

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

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnPoint.position, new Vector3(1f, 1f, 1f));
        }
    }
}
