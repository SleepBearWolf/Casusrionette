using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public List<Instrument> instruments; // ��¡������ͧ�����
    private List<Instrument> correctOrder; // �ӴѺ���١��ͧ
    private int currentStep = 0; // ��鹵͹�Ѩ�غѹ
    public Door door; // ��еٷ��лŴ��ͤ

    void Awake()
    {
        Instance = this;
        correctOrder = new List<Instrument>(instruments); // ��˹��ӴѺ���١��ͧ
        //Shuffle(correctOrder); // �����ӴѺ��������������ҷ��
    }

    public void CheckInstrument(Instrument instrument)
    {
        if (instrument == correctOrder[currentStep])
        {
            currentStep++;
            if (currentStep >= correctOrder.Count)
            {
                door.UnlockDoor(); // ��Ҽ�ҹ�������������Ŵ��ͤ��е�
            }
        }
        else
        {
            Debug.Log("Incorrect! Try again."); // ��ҡ��Դ
            currentStep = 0; // ���������
        }
    }
    /*
    void Shuffle(List<Instrument> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Instrument temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    */
}