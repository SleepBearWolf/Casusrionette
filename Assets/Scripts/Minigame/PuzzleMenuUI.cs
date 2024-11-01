using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject puzzleMenuUI; // UI �ͧ��ҹ���
    [SerializeField] private GameObject puzzlePromptUI; // UI �͡��顴���� G �����Դ��ҹ���
    private bool isPlayerInPuzzleArea = false; // ����Ҽ���������㹺���ǳ��ҹ����������
    private bool isPuzzleOpen = false; // ʶҹТͧ��ҹ����Դ���ͻԴ

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
        // ��Ǩ�ͺ��Ҽ�������������ҹ���
        if (collision.CompareTag("Player"))
        {
            isPlayerInPuzzleArea = true;
            puzzlePromptUI.SetActive(true); // �ʴ� UI �͡��顴 G
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
