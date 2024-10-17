using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject minigamePrefab; // Prefab �ͧ minigame �������� Inspector
    public Transform dropPoint; // �ش���������д�ͻ�͡��

    // Prefabs �ͧ�������ҧ�
    public GameObject moneyPrefab;
    public GameObject diamondPrefab;
    public GameObject goldPrefab;
    public GameObject gemPrefab;

    // �ѵ�ҡ�ô�ͻ�ͧ��������Ъ�Դ
    [Range(0, 100)] public float moneyDropRate = 50f;
    [Range(0, 100)] public float diamondDropRate = 20f;
    [Range(0, 100)] public float goldDropRate = 20f;
    [Range(0, 100)] public float gemDropRate = 10f;

    private bool isNearObject = false;
    private bool minigameCompleted = false;

    void Update()
    {
        // ��Ǩ�ͺ��Ҽ����蹡����� "E" ���������� Object
        if (isNearObject && Input.GetKeyDown(KeyCode.E))
        {
            StartMinigame();
        }
    }

    // �ѧ��ѹ����� Minigame
    void StartMinigame()
    {
        if (minigamePrefab != null)
        {
            // ���ҧ Minigame �����
            GameObject minigameInstance = Instantiate(minigamePrefab);
            MinigameController minigameController = minigameInstance.GetComponent<MinigameController>();

            // ����� Minigame ����� callback ������������
            minigameController.StartMinigame(OnMinigameComplete);
        }
    }

    // �ѧ��ѹ callback ����� Minigame �������
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

    // �ѧ��ѹ������д�ͻ�����
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

    // ��Ǩ�ͺ��Ҽ������������ Object �������
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



