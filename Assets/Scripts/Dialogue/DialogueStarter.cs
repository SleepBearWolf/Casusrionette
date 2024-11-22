using UnityEngine;
using DialogueEditor; 

public class DialogueStarter : MonoBehaviour
{
    public NPCConversation conversation; 

    public void StartDialogue()
    {
        if (conversation != null)
        {
            ConversationManager.Instance.StartConversation(conversation);
        }
        else
        {
            Debug.LogWarning("Conversation not assigned in DialogueStarter!");
        }
    }
}
