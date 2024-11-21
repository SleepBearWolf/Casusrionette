using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked = true; // สถานะของประตู
    public string nextSceneName; // ชื่อของซีนถัดไป

    void OnMouseDown() // เมื่อผู้เล่นคลิกที่ประตู
    {
        if (!isLocked)
        {
            LoadNextScene(); // ถ้าประตูไม่ล็อค ให้โหลดซีนถัดไป
        }
        else
        {
            Debug.Log("The door is locked!"); // แจ้งว่าประตูล็อคอยู่
        }
    }

    void LoadNextScene()
    {
        // ใช้ SceneManager เพื่อโหลดซีนถัดไป
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    public void UnlockDoor() // ฟังก์ชันสำหรับปลดล็อคประตู
    {
        isLocked = false;
        Debug.Log("The door has been unlocked!");
    }
}
