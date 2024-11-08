using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject rewardItem;  // ����������ʴ����������
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
                ClearMatchingPieces(); // ��ҧ�����ǹ���Ѻ���ѹ��
            }
            else if (pieces.Count == 0) // ����ͪ����ǹ����������
            {
                ShowWinUI();  // �ʴ� Win UI
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !gameComplete)
        {
            Shuffle(); // ���������ǹ��������͡����� R
        }
    }

    private bool CheckMatching()
    {
        // �ѧ��ѹ�����㹡�õ�Ǩ�ͺ����ժ����ǹ�˹���Ѻ������������
        // ������ҧ�� ����Ҫ����ǹ�Դ�ѹ�ժ�������͹�ѹ�������
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
        // �ѧ��ѹ���зӡ��ź�����ǹ���Ѻ���ѹ���͡�ҡ˹�Ҩ�
        for (int i = 0; i < pieces.Count - 1; i++)
        {
            if (pieces[i] != null && pieces[i + 1] != null && pieces[i].name == pieces[i + 1].name)
            {
                Destroy(pieces[i].gameObject);
                Destroy(pieces[i + 1].gameObject);
                pieces.RemoveAt(i);
                pieces.RemoveAt(i); // ź��Ƿ��Ѵ���ѧź��ǻѨ�غѹ
                break;
            }
        }
    }

    private void ShowWinUI()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);  // �ʴ� Win UI
        }

        if (rewardItem != null)
        {
            rewardItem.SetActive(true);  // �ʴ�������ҧ���
        }

        if (puzzleMenuUI != null)
        {
            puzzleMenuUI.StopTimer();  // ��ش�Ѻ��������ͼ����蹪��
        }

        gameComplete = true;  // ��駤��ʶҹ��������������ó�
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        // ���������ǹ���������͹䢷���˹�
    }
}
