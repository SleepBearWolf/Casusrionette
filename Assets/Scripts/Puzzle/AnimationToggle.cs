using UnityEngine;

public class AnimationToggle : MonoBehaviour
{
    public Animator animator; // ตัวแปรสำหรับ Animator
    private bool isAnimation1Playing = true; // สถานะของอนิเมชัน

    // ฟังก์ชันที่เรียกเมื่อกดปุ่ม
    public void ToggleAnimation()
    {
        if (isAnimation1Playing)
        {
            animator.SetTrigger("PlayAnimation1"); // เล่นอนิเมชัน 1
        }
        else
        {
            animator.SetTrigger("PlayAnimation2"); // เล่นอนิเมชัน 2
        }

        isAnimation1Playing = !isAnimation1Playing; // เปลี่ยนสถานะ
    }
}