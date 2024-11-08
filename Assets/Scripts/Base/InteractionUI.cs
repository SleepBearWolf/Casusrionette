using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public GameObject interactionUI;  // ��ҧ�ԧ��ѧ UI �����ʴ������������
    public float interactionDistance = 2f;  // ���з������ö interact ��
    private GameObject player;  // ��ҧ�ԧ��ѧ��Ǽ�����
    private bool isPlayerNearby = false;  // ��Ǩ�ͺ��Ҽ�������������������

    void Start()
    {
        player = GameObject.FindWithTag("Player");  // �� player �� tag
        interactionUI.SetActive(false);  // ��͹ UI �͹�������
    }

    void Update()
    {
        CheckInteractionDistance();  // ��Ǩ�ͺ������ҧ�����ҧ��������Шش interact
    }

    // �ѧ��ѹ��Ǩ�ͺ������ҧ
    void CheckInteractionDistance()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance)
        {
            if (!isPlayerNearby)
            {
                ShowInteractionUI();  // �ʴ� UI ����ͼ������������
            }
            isPlayerNearby = true;

            // ����ͼ����蹡����� Interact �蹻��� E
            if (Input.GetKeyDown(KeyCode.B))
            {
                Interact();  // ���¡�ѧ��ѹ Interact
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                HideInteractionUI();  // ��͹ UI ����ͼ������Թ�͡�
            }
            isPlayerNearby = false;
        }
    }

    // �ѧ��ѹ�ʴ� UI
    void ShowInteractionUI()
    {
        interactionUI.SetActive(true);
    }

    // �ѧ��ѹ��͹ UI
    void HideInteractionUI()
    {
        interactionUI.SetActive(false);
    }

    // �ѧ��ѹ���ж١���¡����� Interact
    void Interact()
    {
        Debug.Log("Player is interacting...");
        // ����������Ѻ��÷���觷���ͧ��� �� �ٴ��¡Ѻ NPC �����Դ��е�
    }
}
