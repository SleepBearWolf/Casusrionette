using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public GameObject interactionUI;  // อ้างอิงไปยัง UI ที่จะแสดงเมื่อเข้าใกล้
    public float interactionDistance = 2f;  // ระยะที่สามารถ interact ได้
    private GameObject player;  // อ้างอิงไปยังตัวผู้เล่น
    private bool isPlayerNearby = false;  // ตรวจสอบว่าผู้เล่นอยู่ใกล้หรือไม่

    void Start()
    {
        player = GameObject.FindWithTag("Player");  // หา player โดย tag
        interactionUI.SetActive(false);  // ซ่อน UI ตอนเริ่มต้น
    }

    void Update()
    {
        CheckInteractionDistance();  // ตรวจสอบระยะห่างระหว่างผู้เล่นและจุด interact
    }

    // ฟังก์ชันตรวจสอบระยะห่าง
    void CheckInteractionDistance()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance)
        {
            if (!isPlayerNearby)
            {
                ShowInteractionUI();  // แสดง UI เมื่อผู้เล่นอยู่ใกล้
            }
            isPlayerNearby = true;

            // เมื่อผู้เล่นกดปุ่ม Interact เช่นปุ่ม E
            if (Input.GetKeyDown(KeyCode.B))
            {
                Interact();  // เรียกฟังก์ชัน Interact
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                HideInteractionUI();  // ซ่อน UI เมื่อผู้เล่นเดินออกไป
            }
            isPlayerNearby = false;
        }
    }

    // ฟังก์ชันแสดง UI
    void ShowInteractionUI()
    {
        interactionUI.SetActive(true);
    }

    // ฟังก์ชันซ่อน UI
    void HideInteractionUI()
    {
        interactionUI.SetActive(false);
    }

    // ฟังก์ชันที่จะถูกเรียกเมื่อ Interact
    void Interact()
    {
        Debug.Log("Player is interacting...");
        // เพิ่มโค้ดสำหรับการทำสิ่งที่ต้องการ เช่น พูดคุยกับ NPC หรือเปิดประตู
    }
}

