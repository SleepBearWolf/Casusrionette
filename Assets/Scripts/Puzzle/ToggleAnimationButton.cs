using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimationButton : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator; // ลาก Animator ของ GameObject มาใส่ใน Inspector
    [SerializeField] private string animationTriggerName; // ชื่อ Trigger ที่จะใช้ใน Animator
    [SerializeField] private Text buttonText; // ลาก Text UI ของปุ่มมาใส่ใน Inspector

    private bool isAnimationPlaying = false; // สถานะของ Animation

    private void Start()
    {
        UpdateButtonText(); // ตั้งค่าข้อความเริ่มต้นของปุ่ม
    }

    // ฟังก์ชันที่เรียกจากปุ่ม
    public void ToggleAnimation()
    {
        if (targetAnimator == null) return;

        isAnimationPlaying = !isAnimationPlaying; // สลับสถานะการเล่น Animation

        if (isAnimationPlaying)
        {
            targetAnimator.SetTrigger(animationTriggerName); // เริ่ม Animation
        }
        else
        {
            targetAnimator.Play("Idle"); // กลับไปยังสถานะ Idle (หรือสถานะหยุด)
        }

        UpdateButtonText(); // อัปเดตข้อความปุ่ม
    }

    // อัปเดตข้อความในปุ่ม
    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isAnimationPlaying ? "Stop Animation" : "Play Animation";
        }
    }
}
