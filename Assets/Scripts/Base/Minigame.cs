using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    private System.Action<bool> onCompleteCallback;

    public void StartMinigame(System.Action<bool> onComplete)
    {
        onCompleteCallback = onComplete;
        // ����������Ѻ minigame ����ͧ�������������� (�� ���ȹ�, ���Ѻ���)
        // ������ҧ: �ӡ�è��ͧ minigame �������������� 2 �Թҷ�
        StartCoroutine(SimulateMinigame());
    }

    IEnumerator SimulateMinigame()
    {
        yield return new WaitForSeconds(2f); // ���ͧ��������
        bool success = Random.Range(0f, 1f) > 0.5f; // ���͡�� 50% ���м�ҹ minigame

        Debug.Log(success ? "Minigame Passed!" : "Minigame Failed!");

        // �觼��Ѿ���Ѻ��ѧ�к� interact
        onCompleteCallback?.Invoke(success);
    }
}

