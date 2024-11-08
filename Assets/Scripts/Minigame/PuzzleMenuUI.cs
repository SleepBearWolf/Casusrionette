using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class PuzzleMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject puzzleMenuUI;
    [SerializeField] private GameObject puzzlePromptUI;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private TextMeshProUGUI timerText;  // TextMeshPro component for timer display
    [SerializeField] private float puzzleTimeLimit = 30f; // Time limit in seconds

    private bool isPlayerInPuzzleArea = false;
    private bool isPuzzleOpen = false;
    private float timeRemaining;  // Remaining time for countdown
    private Coroutine timerCoroutine;

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

            if (cameraFollow != null)
            {
                cameraFollow.enabled = false;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Start the countdown timer
            timeRemaining = puzzleTimeLimit;
            timerCoroutine = StartCoroutine(CountdownTimer());
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

            if (cameraFollow != null)
            {
                cameraFollow.enabled = true;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Stop the countdown timer
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
        }
    }

    private bool isGameComplete = false;

    public void StopTimer()
    {
        isGameComplete = true;  // ตั้งค่าสถานะว่าเกมจบแล้ว
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);  // หยุดการทำงานของ Coroutine
        }
        timerText.text = Mathf.Ceil(timeRemaining).ToString();  // แสดงเวลาที่เหลืออยู่
    }




    IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0 && !isGameComplete)
        {
            timeRemaining -= Time.unscaledDeltaTime; // ใช้เวลา unscaled เพื่อไม่ให้หยุดเวลาเมื่อ Time.timeScale = 0
            timerText.text = Mathf.Ceil(timeRemaining).ToString();  // Update the timer display
            yield return null;
        }

        // เมื่อเวลาหมดหรือตัวต่อเสร็จ ให้ปิดเมนู
        if (!isGameComplete)
        {
            DeactivateMenu();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPuzzleArea = true;
            puzzlePromptUI.SetActive(true);
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
