using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimationButton : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator; // �ҡ Animator �ͧ GameObject ������ Inspector
    [SerializeField] private string animationTriggerName; // ���� Trigger ������� Animator
    [SerializeField] private Text buttonText; // �ҡ Text UI �ͧ���������� Inspector

    private bool isAnimationPlaying = false; // ʶҹТͧ Animation

    private void Start()
    {
        UpdateButtonText(); // ��駤�Ң�ͤ���������鹢ͧ����
    }

    // �ѧ��ѹ������¡�ҡ����
    public void ToggleAnimation()
    {
        if (targetAnimator == null) return;

        isAnimationPlaying = !isAnimationPlaying; // ��ѺʶҹС����� Animation

        if (isAnimationPlaying)
        {
            targetAnimator.SetTrigger(animationTriggerName); // ����� Animation
        }
        else
        {
            targetAnimator.Play("Idle"); // ��Ѻ��ѧʶҹ� Idle (����ʶҹ���ش)
        }

        UpdateButtonText(); // �ѻവ��ͤ�������
    }

    // �ѻവ��ͤ���㹻���
    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isAnimationPlaying ? "Stop Animation" : "Play Animation";
        }
    }
}
