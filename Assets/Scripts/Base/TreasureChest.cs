using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class TreasureChest : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject gemPrefab;
    public GameObject goldPrefab;
    public GameObject diamondPrefab;

    public Transform dropPoint;

    public float coinDropRate = 50f;
    public float gemDropRate = 30f;
    public float goldDropRate = 15f;
    public float diamondDropRate = 5f;

    private bool isNearChest = false;
    private bool chestOpened = false;

    void Update()
    {
        if (isNearChest && Input.GetKeyDown(KeyCode.E))
        {
            if (!chestOpened)
            {
                // Trigger the minigame
                MinigameManager.Instance.StartMinigame(OnMinigameComplete);
            }
        }
    }

    void OnMinigameComplete(bool success)
    {
        if (success)
        {
            OpenChest();
        }
        else
        {
            Debug.Log("Minigame failed, chest remains closed.");
        }
    }

    void OpenChest()
    {
        chestOpened = true;
        Debug.Log("Chest opened!");
        DropItem();
    }

    void DropItem()
    {
        float randomValue = Random.Range(0f, 100f);
        GameObject itemToDrop = null;

        if (randomValue <= diamondDropRate)
        {
            itemToDrop = diamondPrefab;
        }
        else if (randomValue <= diamondDropRate + goldDropRate)
        {
            itemToDrop = goldPrefab;
        }
        else if (randomValue <= diamondDropRate + goldDropRate + gemDropRate)
        {
            itemToDrop = gemPrefab;
        }
        else
        {
            itemToDrop = coinPrefab;
        }

        if (itemToDrop != null)
        {
            GameObject droppedItem = Instantiate(itemToDrop, dropPoint.position, Quaternion.identity);
            droppedItem.GetComponent<Item>().StartBounce();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearChest = true;
            Debug.Log("Player near chest");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearChest = false;
            Debug.Log("Player left chest");
        }
    }
}




