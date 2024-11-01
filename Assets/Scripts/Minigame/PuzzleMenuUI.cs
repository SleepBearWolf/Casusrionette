using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject puzzleMenuUI; // UI ของร้านค้า
    [SerializeField] private GameObject puzzlePromptUI; // UI บอกให้กดปุ่ม G เพื่อเปิดร้านค้า
    private bool isPlayerInPuzzleArea = false; // เช็คว่าผู้เล่นอยู่ในบริเวณร้านค้าหรือไม่
    private bool isPuzzleOpen = false; // สถานะของร้านค้าเปิดหรือปิด

    private void Update()
    {
        if (isPlayerInPuzzleArea && Input.GetKeyDown(KeyCode.R))
        {
            isPuzzleOpen = !isPuzzleOpen;

            if (isPuzzleOpen)
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
        if (puzzleMenuUI != null && puzzlePromptUI != null)
        {
            Time.timeScale = 0;
            AudioListener.pause = false;
            puzzleMenuUI.SetActive(true);
            puzzlePromptUI.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }


    public void DeactivateMenu()
    {
        if (puzzleMenuUI != null && puzzlePromptUI != null)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            puzzleMenuUI.SetActive(false);
            puzzlePromptUI.SetActive(true);
            isPuzzleOpen = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าผู้เล่นเข้าใกล้ร้านค้า
        if (collision.CompareTag("Player"))
        {
            isPlayerInPuzzleArea = true;
            puzzlePromptUI.SetActive(true); // แสดง UI บอกให้กด G
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPuzzleArea = false;
            if (puzzlePromptUI != null)
            {
                puzzlePromptUI.SetActive(false);
            }
            if (isPuzzleOpen)
            {
                DeactivateMenu();
            }
        }
    }
}
