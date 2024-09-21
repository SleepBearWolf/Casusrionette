using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{
    public GameObject toolPanel;   // Panel ที่จะแสดงหรือซ่อน
    private bool isPanelVisible = false;  // ตัวบ่งชี้ว่าตอนนี้ Panel แสดงหรือไม่
    private bool isAnimating = false;  // ตัวบ่งชี้ว่าตอนนี้ Panel กำลังเคลื่อนไหวหรือไม่

    private Vector3 hiddenPosition;  // ตำแหน่งซ่อน
    private Vector3 shownPosition;   // ตำแหน่งแสดง

    public Button toolButton;   // ปุ่มสำหรับแสดงหรือซ่อน Panel

    void Start()
    {
        // ตั้งค่าตำแหน่งซ่อนและแสดง
        hiddenPosition = toolPanel.transform.localPosition;
        shownPosition = new Vector3(hiddenPosition.x, hiddenPosition.y + 300, hiddenPosition.z);

        // ตรวจสอบการเชื่อมโยงใน Inspector
        if (toolButton != null && toolPanel != null)
        {
            toolButton.onClick.AddListener(ToggleToolPanel);  // เชื่อมต่อการคลิกปุ่มเพื่อสลับการแสดง Tool Panel
        }
        else
        {
            Debug.LogError("ToolButton or ToolPanel is not connected in Inspector!");
        }
    }

    // ฟังก์ชันสำหรับสลับการแสดงผลของ Tool Panel
    public void ToggleToolPanel()
    {
        if (isAnimating) return;  // ถ้า Panel กำลังเคลื่อนไหว ให้หยุดทำงานไปก่อน

        if (isPanelVisible)
        {
            HideToolMenu();  // ซ่อน Panel
        }
        else
        {
            ShowToolMenu();  // แสดง Panel
        }
    }

    // ฟังก์ชันแสดง Tool Panel (เรียกจากสคริปต์อื่นได้)
    public void ShowToolMenu()
    {
        if (!isAnimating)  // ตรวจสอบว่าไม่กำลังเคลื่อนไหวอยู่
        {
            isAnimating = true;  // เริ่มการเคลื่อนไหว
            LeanTween.moveLocal(toolPanel, shownPosition, 0.5f).setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    isAnimating = false;  // การเคลื่อนไหวเสร็จสิ้น
                    isPanelVisible = true;  // อัพเดทสถานะ Panel
                });
            Debug.Log("Tool Menu is now visible");
        }
    }

    // ฟังก์ชันซ่อน Tool Panel (เรียกจากสคริปต์อื่นได้)
    public void HideToolMenu()
    {
        if (!isAnimating)  // ตรวจสอบว่าไม่กำลังเคลื่อนไหวอยู่
        {
            isAnimating = true;  // เริ่มการเคลื่อนไหว
            LeanTween.moveLocal(toolPanel, hiddenPosition, 0.5f).setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    isAnimating = false;  // การเคลื่อนไหวเสร็จสิ้น
                    isPanelVisible = false;  // อัพเดทสถานะ Panel
                });
            Debug.Log("Tool Menu is now hidden");
        }
    }
}
