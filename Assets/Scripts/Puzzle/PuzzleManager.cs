using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public List<Instrument> instruments; // รายการเครื่องดนตรี
    private List<Instrument> correctOrder; // ลำดับที่ถูกต้อง
    private int currentStep = 0; // ขั้นตอนปัจจุบัน
    public Door door; // ประตูที่จะปลดล็อค

    void Awake()
    {
        Instance = this;
        correctOrder = new List<Instrument>(instruments); // กำหนดลำดับที่ถูกต้อง
        //Shuffle(correctOrder); // สุ่มลำดับเพื่อเพิ่มความท้าทาย
    }

    public void CheckInstrument(Instrument instrument)
    {
        if (instrument == correctOrder[currentStep])
        {
            currentStep++;
            if (currentStep >= correctOrder.Count)
            {
                door.UnlockDoor(); // ถ้าผ่านทั้งหมดแล้วให้ปลดล็อคประตู
            }
        }
        else
        {
            Debug.Log("Incorrect! Try again."); // ถ้ากดผิด
            currentStep = 0; // เริ่มใหม่
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