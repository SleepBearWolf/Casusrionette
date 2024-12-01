using UnityEngine;
using DialogueEditor;
using System.Collections;

public class DialogTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public NPCConversation npcConversation; // ��ʹ��ҷ���ͧ����ʴ�
    private bool hasEnteredRoom = false; // ��Ǩ�ͺ������ʴ���ʹ������������ѧ

    [Header("Delay Settings")]
    public float delayTime = 0f; // ��������˹�ǧ (�Թҷ�) ��͹�ʴ���ʹ���

    /// <summary>
    /// Trigger Dialogue ����ͼ�������������ͧ
    /// </summary>
    public void TriggerDialogue()
    {
        // ��Ǩ�ͺ������ʴ���ʹ�������������ѧ
        if (hasEnteredRoom) return;

        // ��˹�������ʴ���ʹ�������
        hasEnteredRoom = true;

        // ����� Coroutine ����˹�ǧ��������ʴ���ʹ���
        StartCoroutine(TriggerDialogueWithDelay());
    }

    /// <summary>
    /// Coroutine ����Ѻ˹�ǧ����㹡���ʴ���ʹ���
    /// </summary>
    private IEnumerator TriggerDialogueWithDelay()
    {
        // ˹�ǧ����
        yield return new WaitForSeconds(delayTime);

        // �������ʹ���
        if (npcConversation != null)
        {
            ConversationManager.Instance.StartConversation(npcConversation);

            // �ʴ�����������������������ͺ��
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// ����ʶҹ������������ö�ʴ���ʹ��������� (��Ҩ���)
    /// </summary>
    public void ResetDialogue()
    {
        hasEnteredRoom = false;
    }
}
