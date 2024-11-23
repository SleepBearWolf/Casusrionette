using UnityEngine;

public class DrawerDrag : MonoBehaviour
{
    private Vector3 offset;         // ������ҧ�����ҧ���˹������Ѻ���˹���鹪ѡ
    private bool isDragging = false; // ʶҹС���ҡ

    public float minY = 0f;         // �ͺࢵ�������͹��ҹ��ҧ (���˹觻Դ�ش)
    public float maxY = 2f;         // �ͺࢵ�������͹��ҹ�� (���˹��Դ�ش)
    public AudioClip dragSound;     // ���§������ҡ��鹪ѡ
    private AudioSource audioSource; // ���������§

    void Start()
    {
        // ���� AudioSource ����ѧ�����
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = dragSound;
        audioSource.playOnAwake = false; // �Դ���§����ѵ��ѵ�
    }

    void OnMouseDown()
    {
        // ���������ҡ��Фӹǳ offset
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ������§������ҡ
        if (dragSound && audioSource)
        {
            audioSource.loop = true; // ��蹫�������ҧ�ҡ
            audioSource.Play();
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // �ӹǳ���˹����������ͤ�������㹢ͺࢵ
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY); // ��ͤ���˹�᡹ Y
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        // ��ش�ҡ
        isDragging = false;

        // ��ش���§����ͻ���������
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
