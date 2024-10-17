using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    private System.Action<bool> onCompleteCallback;

    public void StartMinigame(System.Action<bool> onComplete)
    {
        onCompleteCallback = onComplete;
        // แสดง Minigame UI หรือเริ่มการเล่นเกม
        Debug.Log("Minigame started");

        // สำหรับการทดสอบ ใช้การสุ่มผลลัพธ์ (สมมติว่า Minigame ใช้เวลา 2 วินาที)
        StartCoroutine(SimulateMinigame());
    }

    IEnumerator SimulateMinigame()
    {
        yield return new WaitForSeconds(2f); // ระยะเวลา Minigame

        bool success = Random.Range(0f, 1f) > 0.5f; // โอกาสผ่าน 50%
        Debug.Log(success ? "Minigame Passed!" : "Minigame Failed!");

        onCompleteCallback?.Invoke(success);

        // ทำลาย Minigame UI หลังจากจบ
        Destroy(gameObject);
    }
}


