using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject shopMenuUI; // UI ของร้านค้า
    [SerializeField] private GameObject shopPromptUI; // UI บอกให้กดปุ่ม G เพื่อเปิดร้านค้า
    private bool isPlayerInShopArea = false; // เช็คว่าผู้เล่นอยู่ในบริเวณร้านค้าหรือไม่
    private bool isShopOpen = false; // สถานะของร้านค้าเปิดหรือปิด

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
        Time.timeScale = 0; // หยุดเวลา
        AudioListener.pause = false; // หยุดเสียงในเกม
        shopMenuUI.SetActive(true); // แสดงเมนูร้านค้า
        shopPromptUI.SetActive(false); // ซ่อน UI บอกให้กด G

        Cursor.visible = true; // แสดงเคอร์เซอร์
        Cursor.lockState = CursorLockMode.None; // ปลดล็อคเคอร์เซอร์
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1; // กลับมาใช้เวลาปกติ
        AudioListener.pause = false; // ปล่อยเสียงในเกม
        shopMenuUI.SetActive(false); // ซ่อนเมนูร้านค้า
        shopPromptUI.SetActive(true); // แสดง UI บอกให้กด G
        isShopOpen = false;

        Cursor.visible = false; // ซ่อนเคอร์เซอร์
        Cursor.lockState = CursorLockMode.Locked; // ล็อคเคอร์เซอร์
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าผู้เล่นเข้าใกล้ร้านค้า
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopArea = true;
            shopPromptUI.SetActive(true); // แสดง UI บอกให้กด G
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ตรวจสอบว่าผู้เล่นออกจากบริเวณร้านค้า
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopArea = false;
            shopPromptUI.SetActive(false); // ซ่อน UI บอกให้กด G
            if (isShopOpen)
            {
                DeactivateMenu(); // ปิดเมนูร้านค้าหากยังเปิดอยู่
            }
        }
    }
}
