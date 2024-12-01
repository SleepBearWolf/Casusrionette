using UnityEngine;
using DialogueEditor;
using System.Collections;

public class DialogTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public NPCConversation npcConversation; // บทสนทนาที่ต้องการแสดง
    private bool hasEnteredRoom = false; // ตรวจสอบว่าเคยแสดงบทสนทนาแล้วหรือยัง

    [Header("Delay Settings")]
    public float delayTime = 0f; // ระยะเวลาหน่วง (วินาที) ก่อนแสดงบทสนทนา

    /// <summary>
    /// Trigger Dialogue เมื่อผู้เล่นเข้าสู่ห้อง
    /// </summary>
    public void TriggerDialogue()
    {
        // ตรวจสอบว่าเคยแสดงบทสนทนาไปแล้วหรือยัง
        if (hasEnteredRoom) return;

        // กำหนดว่าเคยแสดงบทสนทนาแล้ว
        hasEnteredRoom = true;

        // เริ่ม Coroutine เพื่อหน่วงเวลาและแสดงบทสนทนา
        StartCoroutine(TriggerDialogueWithDelay());
    }

    /// <summary>
    /// Coroutine สำหรับหน่วงเวลาในการแสดงบทสนทนา
    /// </summary>
    private IEnumerator TriggerDialogueWithDelay()
    {
        // หน่วงเวลา
        yield return new WaitForSeconds(delayTime);

        // เริ่มบทสนทนา
        if (npcConversation != null)
        {
            ConversationManager.Instance.StartConversation(npcConversation);

            // แสดงเคอร์เซอร์เพื่อให้ผู้เล่นโต้ตอบได้
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// รีเซ็ตสถานะเพื่อให้สามารถแสดงบทสนทนาใหม่ได้ (ถ้าจำเป็น)
    /// </summary>
    public void ResetDialogue()
    {
        hasEnteredRoom = false;
    }
}
