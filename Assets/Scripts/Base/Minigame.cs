using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    private System.Action<bool> onCompleteCallback;

    public void StartMinigame(System.Action<bool> onComplete)
    {
        onCompleteCallback = onComplete;
        // เพิ่มโค้ดสำหรับ minigame ที่ต้องการให้ผู้เล่นเล่น (เช่น ปริศนา, เกมจับคู่)
        // ตัวอย่าง: ทำการจำลอง minigame ที่เล่นเสร็จภายใน 2 วินาที
        StartCoroutine(SimulateMinigame());
    }

    IEnumerator SimulateMinigame()
    {
        yield return new WaitForSeconds(2f); // จำลองการเล่นเกม
        bool success = Random.Range(0f, 1f) > 0.5f; // มีโอกาส 50% ที่จะผ่าน minigame

        Debug.Log(success ? "Minigame Passed!" : "Minigame Failed!");

        // ส่งผลลัพธ์กลับไปยังระบบ interact
        onCompleteCallback?.Invoke(success);
    }
}

