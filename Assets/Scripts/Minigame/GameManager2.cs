using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject rewardItem;  // ไอเท็มที่จะแสดงเมื่อเกมจบ
    [SerializeField] private PuzzleMenuUI puzzleMenuUI;

    private List<Transform> pieces;
    private int size;
    private bool shuffling = false;
    private bool gameComplete = false;

    private void CreateGamePieces(float gapThickness)
    {
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);
                piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
                                                  +1 - (2 * width * row) - width,
                                                  0);
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
            }
        }
    }

    void Start()
    {
        pieces = new List<Transform>();
        size = 3;
        CreateGamePieces(0.01f);

        if (!shuffling && CheckMatching())
        {
            shuffling = true;
            StartCoroutine(WaitShuffle(0.5f));
        }
    }

    void Update()
    {
        if (!shuffling && !gameComplete)
        {
            if (CheckMatching())
            {
                ClearMatchingPieces(); // ล้างชิ้นส่วนที่จับคู่กันได้
            }
            else if (pieces.Count == 0) // เมื่อชิ้นส่วนทั้งหมดหายไป
            {
                ShowWinUI();  // แสดง Win UI
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !gameComplete)
        {
            Shuffle(); // สุ่มชิ้นส่วนใหม่เมื่อกดปุ่ม R
        }
    }

    private bool CheckMatching()
    {
        // ฟังก์ชันนี้ใช้ในการตรวจสอบว่ามีชิ้นส่วนไหนที่จับคู่ได้หรือไม่
        // ตัวอย่างเช่น เช็คว่าชิ้นส่วนติดกันมีชื่อเหมือนกันหรือไม่
        for (int i = 0; i < pieces.Count - 1; i++)
        {
            if (pieces[i] != null && pieces[i + 1] != null && pieces[i].name == pieces[i + 1].name)
            {
                return true;
            }
        }
        return false;
    }

    private void ClearMatchingPieces()
    {
        // ฟังก์ชันนี้จะทำการลบชิ้นส่วนที่จับคู่กันได้ออกจากหน้าจอ
        for (int i = 0; i < pieces.Count - 1; i++)
        {
            if (pieces[i] != null && pieces[i + 1] != null && pieces[i].name == pieces[i + 1].name)
            {
                Destroy(pieces[i].gameObject);
                Destroy(pieces[i + 1].gameObject);
                pieces.RemoveAt(i);
                pieces.RemoveAt(i); // ลบตัวที่ถัดไปหลังลบตัวปัจจุบัน
                break;
            }
        }
    }

    private void ShowWinUI()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);  // แสดง Win UI
        }

        if (rewardItem != null)
        {
            rewardItem.SetActive(true);  // แสดงไอเท็มรางวัล
        }

        if (puzzleMenuUI != null)
        {
            puzzleMenuUI.StopTimer();  // หยุดจับเวลาเมื่อผู้เล่นชนะ
        }

        gameComplete = true;  // ตั้งค่าสถานะเกมให้เสร็จสมบูรณ์
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        // สุ่มชิ้นส่วนใหม่ตามเงื่อนไขที่กำหนด
    }
}
