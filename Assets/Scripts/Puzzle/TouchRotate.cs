using UnityEngine;

public class TouchRotate : MonoBehaviour
{
    /*
    public float rotationAngle = 90f; // องศาที่จะหมุนในแต่ละครั้ง

    private bool isRotating = false; // ตัวแปรเพื่อป้องกันการหมุนซ้ำระหว่างการทำงาน

    private void OnMouseDown()
    {
        if (!isRotating)
        {
            isRotating = true; // ตั้งค่าสถานะการหมุน
            RotateObject(); // เรียกใช้ฟังก์ชันหมุน
        }
    }

    private void RotateObject()
    {
        // หมุน object 90 องศารอบแกน Z
        transform.Rotate(0, 0, rotationAngle);
        isRotating = false; // ปลดล็อกสถานะเพื่อให้สามารถหมุนใหม่ได้
    }
    */

    private void OnMouseDown()
    {
        if (!GameControl.youWin)
            transform.Rotate(0, 0, 90f);
    }
}
