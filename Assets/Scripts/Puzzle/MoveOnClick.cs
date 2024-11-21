using UnityEngine;

public class MoveOnClick : MonoBehaviour
{
    public float moveDistance = 2f; // ระยะทางที่ Object จะขยับ
    public float moveSpeed = 2f;   // ความเร็วในการขยับ
    private Vector3 targetPosition; // ตำแหน่งเป้าหมายที่จะขยับไป
    private Vector3 initialPosition; // ตำแหน่งเริ่มต้นของ Object

    private bool isMoving = false; // สถานะการขยับ
    private bool isMoved = false;  // สถานะว่าถูกขยับแล้วหรือยัง

    void Start()
    {
        initialPosition = transform.position; // บันทึกตำแหน่งเริ่มต้น
        targetPosition = initialPosition; // กำหนดตำแหน่งเป้าหมายเริ่มต้น
    }

    void OnMouseDown()
    {
        if (isMoving) return; // ป้องกันการกดขณะกำลังขยับ

        if (isMoved)
        {
            // ถ้าถูกขยับแล้ว ให้กลับไปตำแหน่งเริ่มต้น
            targetPosition = initialPosition;
            isMoved = false;
        }
        else
        {
            // สุ่มทิศทาง (ซ้ายหรือขวา) และกำหนดตำแหน่งเป้าหมาย
            int direction = Random.Range(0, 2) == 0 ? -1 : 1;
            targetPosition = transform.position + new Vector3(moveDistance * direction, 0, 0);
            isMoved = true;
        }

        isMoving = true; // เปิดสถานะการขยับ
    }

    void Update()
    {
        if (isMoving)
        {
            // ขยับ Object เข้าหาเป้าหมาย
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // ตรวจสอบว่า Object ถึงตำแหน่งเป้าหมายหรือยัง
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // หยุดการขยับเมื่อถึงเป้าหมาย
            }
        }
    }
}
