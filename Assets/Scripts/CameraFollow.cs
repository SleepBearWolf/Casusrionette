using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // กำหนดให้กล้องติดตามผู้เล่น
    public Vector3 offset;   // กำหนดค่าการเคลื่อนที่จากตำแหน่งผู้เล่น
    public float smoothSpeed = 0.125f; // ความเร็วในการเคลื่อนที่ของกล้อง

    [SerializeField] private float minX, maxX, minY, maxY; // กำหนดขอบเขตกล้อง

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX); // ล็อกการเคลื่อนไหวของกล้องในขอบเขตที่กำหนด
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
