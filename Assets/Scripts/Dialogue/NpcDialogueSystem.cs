using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class NpcDialogueSystem : MonoBehaviour
{
    public Transform player;
    public Vector2 overlapBoxSize = new Vector2(2f, 2f);
    public KeyCode interactKey = KeyCode.E;

    [Header("Task System")]
    public TaskManager taskManager;  // Reference to the TaskManager system

    [Header("Dialogue System")]
    public NPCConversation npcConversation;
    public string taskName;  // Add task name to check which task is related to this NPC
    public ItemBaseData requiredItem;  // Item to check if player needs to give it to NPC

    private bool isTalking = false;
    private bool playerInRange = false;

    private void Update()
    {
        // Detect if player is within range
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

        // Start the dialogue when the player presses the interact key
        if (playerInRange && Input.GetKeyDown(interactKey) && !isTalking)
        {
            StartDialogue();
        }

        // End dialogue if player leaves the range
        if (!playerInRange && isTalking)
        {
            EndDialogue();
        }

        // Show the mouse if the dialogue is showing options
        if (isTalking && IsShowingOptions())
        {
            ShowMouse();
        }
        else if (isTalking && !IsShowingOptions())
        {
            HideMouse();
        }
    }

    // Start the dialogue and check if the task can be started
    private void StartDialogue()
    {
        isTalking = true;
        if (npcConversation != null)
        {
            ConversationManager.Instance.StartConversation(npcConversation);
        }

        // Unlock and show the mouse when the dialogue starts
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Check if task is available and if required items are available
        if (taskManager != null && requiredItem != null)
        {
            if (taskManager.HasTask(taskName) && player.GetComponent<PlayerInventory>().HasItem(requiredItem, 1))
            {
                // If player has the required item, update task progress
                taskManager.UpdateTaskProgress(taskName, 1);
                Debug.Log("Task progress updated for: " + taskName);
            }
        }
    }

    // End the dialogue and check task completion
    private void EndDialogue()
    {
        isTalking = false;
        ConversationManager.Instance.EndConversation();

        // Lock and hide the mouse after the dialogue ends
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Check if task is completed
        if (taskManager != null)
        {
            taskManager.CheckTaskCompletion(taskName); // ตรวจสอบภารกิจเมื่อการสนทนาจบลง
        }
    }

    // Show the mouse during dialogue options
    private void ShowMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Hide the mouse when options are not shown
    private void HideMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Check if dialogue options are being displayed
    private bool IsShowingOptions()
    {
        return ConversationManager.Instance != null && ConversationManager.Instance.IsShowingOptions();
    }

    // Draw gizmos to show the interaction area in the scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, overlapBoxSize);
    }
}
