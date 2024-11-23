using UnityEngine;

public class AnimationToggle : MonoBehaviour
{
    public Animator animator; // ���������Ѻ Animator
    private bool isAnimation1Playing = true; // ʶҹТͧ͹����ѹ

    // �ѧ��ѹ������¡����͡�����
    public void ToggleAnimation()
    {
        if (isAnimation1Playing)
        {
            animator.SetTrigger("PlayAnimation1"); // ���͹����ѹ 1
        }
        else
        {
            animator.SetTrigger("PlayAnimation2"); // ���͹����ѹ 2
        }

        isAnimation1Playing = !isAnimation1Playing; // ����¹ʶҹ�
    }
}