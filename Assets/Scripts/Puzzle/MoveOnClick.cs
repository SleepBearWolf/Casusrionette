using UnityEngine;

public class MoveOnClick : MonoBehaviour
{
    public float moveDistance = 2f; // ���зҧ��� Object �Т�Ѻ
    public float moveSpeed = 2f;   // ��������㹡�â�Ѻ
    private Vector3 targetPosition; // ���˹�������·��Т�Ѻ�
    private Vector3 initialPosition; // ���˹�������鹢ͧ Object

    private bool isMoving = false; // ʶҹС�â�Ѻ
    private bool isMoved = false;  // ʶҹ���Ҷ١��Ѻ���������ѧ

    void Start()
    {
        initialPosition = transform.position; // �ѹ�֡���˹��������
        targetPosition = initialPosition; // ��˹����˹���������������
    }

    void OnMouseDown()
    {
        if (isMoving) return; // ��ͧ�ѹ��á���С��ѧ��Ѻ

        if (isMoved)
        {
            // ��Ҷ١��Ѻ���� ����Ѻ仵��˹��������
            targetPosition = initialPosition;
            isMoved = false;
        }
        else
        {
            // ������ȷҧ (�������͢��) ��С�˹����˹��������
            int direction = Random.Range(0, 2) == 0 ? -1 : 1;
            targetPosition = transform.position + new Vector3(moveDistance * direction, 0, 0);
            isMoved = true;
        }

        isMoving = true; // �ԴʶҹС�â�Ѻ
    }

    void Update()
    {
        if (isMoving)
        {
            // ��Ѻ Object ������������
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // ��Ǩ�ͺ��� Object �֧���˹�������������ѧ
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // ��ش��â�Ѻ����Ͷ֧�������
            }
        }
    }
}
