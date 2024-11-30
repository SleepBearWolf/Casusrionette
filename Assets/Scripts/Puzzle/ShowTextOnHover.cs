using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject textUI; // �ҡ Text UI ����ͧ����ʴ������� Inspector

    private void Awake()
    {
        if (textUI != null)
        {
            textUI.SetActive(false); // ��͹ Text UI �͹�������
        }
    }

    // �ѧ��ѹ���ж١���¡������������价�����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textUI != null)
        {
            textUI.SetActive(true); // �ʴ� Text UI
        }
    }

    // �ѧ��ѹ���ж١���¡�����������͡�ҡ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (textUI != null)
        {
            textUI.SetActive(false); // ��͹ Text UI
        }
    }
}
