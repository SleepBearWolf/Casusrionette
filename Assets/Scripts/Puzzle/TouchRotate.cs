using UnityEngine;

public class TouchRotate : MonoBehaviour
{
    /*
    public float rotationAngle = 90f; // ͧ�ҷ�����ع����Ф���

    private bool isRotating = false; // ��������ͻ�ͧ�ѹ�����ع��������ҧ��÷ӧҹ

    private void OnMouseDown()
    {
        if (!isRotating)
        {
            isRotating = true; // ��駤��ʶҹС����ع
            RotateObject(); // ���¡��ѧ��ѹ��ع
        }
    }

    private void RotateObject()
    {
        // ��ع object 90 ͧ���ͺ᡹ Z
        transform.Rotate(0, 0, rotationAngle);
        isRotating = false; // �Ŵ��͡ʶҹ������������ö��ع������
    }
    */

    private void OnMouseDown()
    {
        if (!GameControl.youWin)
            transform.Rotate(0, 0, 90f);
    }
}
