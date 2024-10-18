using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class CollisionTextTrigger : MonoBehaviour
{
    public Transform player;  
    public Vector2 overlapBoxSize = new Vector2(2f, 2f); 

    [Header("Dialogue System")]
    public NPCConversation npcConversation;  

    private bool isTalking = false;  
    private bool playerInRange = false;  

    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, overlapBoxSize, 0f);
        playerInRange = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == player)  
            {
                playerInRange = true;
                break;
            }
        }

        if (playerInRange && !isTalking)
        {
            StartDialogue(); 
        }

        if (!playerInRange && isTalking)
        {
            EndDialogue(); 
        }

        if (isTalking && IsShowingOptions())
        {
            ShowMouse();
        }
        else if (isTalking && !IsShowingOptions())
        {
            HideMouse();
        }
    }

    private void StartDialogue()
    {
        isTalking = true;

        if (npcConversation != null)
        {
            ConversationManager.Instance.StartConversation(npcConversation);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EndDialogue()
    {
        isTalking = false;

        ConversationManager.Instance.EndConversation();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsShowingOptions()
    {
        return ConversationManager.Instance != null && ConversationManager.Instance.IsShowingOptions();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, overlapBoxSize);
    }
}
