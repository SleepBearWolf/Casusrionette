using UnityEngine;

public class DrawerDrag : MonoBehaviour
{
    private Vector3 offset;         // ความต่างระหว่างตำแหน่งเมาส์กับตำแหน่งลิ้นชัก
    private bool isDragging = false; // สถานะการลาก

    public float minY = 0f;         // ขอบเขตการเลื่อนด้านล่าง (ตำแหน่งปิดสุด)
    public float maxY = 2f;         // ขอบเขตการเลื่อนด้านบน (ตำแหน่งเปิดสุด)
    public AudioClip dragSound;     // เสียงเมื่อลากลิ้นชัก
    private AudioSource audioSource; // ตัวเล่นเสียง

    void Start()
    {
        // เพิ่ม AudioSource ถ้ายังไม่มี
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = dragSound;
        audioSource.playOnAwake = false; // ปิดเสียงเล่นอัตโนมัติ
    }

    void OnMouseDown()
    {
        // เริ่มการลากและคำนวณ offset
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // เล่นเสียงเมื่อลาก
        if (dragSound && audioSource)
        {
            audioSource.loop = true; // เล่นซ้ำระหว่างลาก
            audioSource.Play();
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // คำนวณตำแหน่งใหม่และล็อคให้อยู่ในขอบเขต
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY); // ล็อคตำแหน่งแกน Y
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        // หยุดลาก
        isDragging = false;

        // หยุดเสียงเมื่อปล่อยเมาส์
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
