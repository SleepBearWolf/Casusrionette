using UnityEngine;
using UnityEngine.UI;

public class WorldToolUI : MonoBehaviour
{
    public string requiredTool; // ชื่อเครื่องมือที่จำเป็น
    private PlayerItems playerItems; // ตัวอ้างอิงถึง PlayerItems
    public Button toolButton; // ปุ่มที่ใช้ใน UI

    private void Start()
    {
        playerItems = FindObjectOfType<PlayerItems>(); // ค้นหา PlayerItems ในฉาก

        // ตรวจสอบการตั้งค่า PlayerItems
        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกตั้งค่าใน WorldToolUI!");
        }

        // ตรวจสอบว่ามีการตั้งค่าปุ่มใน Inspector
        if (toolButton != null)
        {
            toolButton.onClick.AddListener(OnButtonClick); // เพิ่ม Listener สำหรับปุ่ม
        }
        else
        {
            Debug.LogError("Button ไม่ถูกตั้งค่าใน WorldToolUI!");
        }
    }

    // ฟังก์ชันสำหรับการคลิกปุ่ม
    public void OnButtonClick()
    {
        if (playerItems != null && playerItems.HasTool(requiredTool))
        {
            Debug.Log("ใช้เครื่องมือถูกต้อง! ลบ UI");
            Destroy(gameObject); // ลบ UI เมื่อใช้เครื่องมือถูกต้อง
        }
        else
        {
            Debug.Log("เครื่องมือที่ใช้ไม่ถูกต้อง! ไม่สามารถลบ UI ได้");
        }
    }
}
