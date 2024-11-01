using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject shopMenuUI; // UI �ͧ��ҹ���
    [SerializeField] private GameObject shopPromptUI; // UI �͡��顴���� G �����Դ��ҹ���
    private bool isPlayerInShopArea = false; // ����Ҽ���������㹺���ǳ��ҹ����������
    private bool isShopOpen = false; // ʶҹТͧ��ҹ����Դ���ͻԴ

    private void Update()
    {
        if (isPlayerInShopArea && Input.GetKeyDown(KeyCode.G))
        {
            isShopOpen = !isShopOpen;

            if (isShopOpen)
            {
                ActivateMenu();
            }
            else
            {
                DeactivateMenu();
            }
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0; // ��ش����
        AudioListener.pause = false; // ��ش���§���
        shopMenuUI.SetActive(true); // �ʴ�������ҹ���
        shopPromptUI.SetActive(false); // ��͹ UI �͡��顴 G

        Cursor.visible = true; // �ʴ���������
        Cursor.lockState = CursorLockMode.None; // �Ŵ��ͤ��������
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1; // ��Ѻ�������һ���
        AudioListener.pause = false; // ��������§���
        shopMenuUI.SetActive(false); // ��͹������ҹ���
        shopPromptUI.SetActive(true); // �ʴ� UI �͡��顴 G
        isShopOpen = false;

        Cursor.visible = false; // ��͹��������
        Cursor.lockState = CursorLockMode.Locked; // ��ͤ��������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��Ǩ�ͺ��Ҽ�������������ҹ���
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopArea = true;
            shopPromptUI.SetActive(true); // �ʴ� UI �͡��顴 G
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ��Ǩ�ͺ��Ҽ������͡�ҡ����ǳ��ҹ���
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopArea = false;
            shopPromptUI.SetActive(false); // ��͹ UI �͡��顴 G
            if (isShopOpen)
            {
                DeactivateMenu(); // �Դ������ҹ����ҡ�ѧ�Դ����
            }
        }
    }
}
