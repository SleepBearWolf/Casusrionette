using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject textUI; // ลาก Text UI ที่ต้องการแสดงมาใส่ใน Inspector

    private void Awake()
    {
        if (textUI != null)
        {
            textUI.SetActive(false); // ซ่อน Text UI ตอนเริ่มเกม
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อเมาส์ชี้ไปที่ปุ่ม
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textUI != null)
        {
            textUI.SetActive(true); // แสดง Text UI
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อเมาส์ออกจากปุ่ม
    public void OnPointerExit(PointerEventData eventData)
    {
        if (textUI != null)
        {
            textUI.SetActive(false); // ซ่อน Text UI
        }
    }
}
