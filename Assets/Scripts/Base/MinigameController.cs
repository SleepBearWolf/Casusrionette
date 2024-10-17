using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    private System.Action<bool> onCompleteCallback;

    public void StartMinigame(System.Action<bool> onComplete)
    {
        onCompleteCallback = onComplete;
        // �ʴ� Minigame UI �����������������
        Debug.Log("Minigame started");

        // ����Ѻ��÷��ͺ �����������Ѿ�� (�������� Minigame ������ 2 �Թҷ�)
        StartCoroutine(SimulateMinigame());
    }

    IEnumerator SimulateMinigame()
    {
        yield return new WaitForSeconds(2f); // �������� Minigame

        bool success = Random.Range(0f, 1f) > 0.5f; // �͡�ʼ�ҹ 50%
        Debug.Log(success ? "Minigame Passed!" : "Minigame Failed!");

        onCompleteCallback?.Invoke(success);

        // ����� Minigame UI ��ѧ�ҡ��
        Destroy(gameObject);
    }
}


