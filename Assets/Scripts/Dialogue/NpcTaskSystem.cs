using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTaskSystem : MonoBehaviour
{
    public List<TaskData> npcTasks;
    private TaskData currentTask;

    public PlayerInventory playerInventory;

    private void Start()
    {
        AssignNextTask();
    }

    public void CheckTaskCompletion()
    {
        if (currentTask == null || currentTask.isCompleted)
        {
            Debug.Log("No task assigned or task already completed.");
            return;
        }

        if (playerInventory.HasItem(currentTask.requiredItem, currentTask.requiredAmount))
        {
            playerInventory.RemoveItem(currentTask.requiredItem, currentTask.requiredAmount);
            currentTask.isCompleted = true;
            Debug.Log("Task completed: " + currentTask.taskName);
            RewardPlayer(currentTask.rewardItem);

            AssignNextTask();
        }
        else
        {
            Debug.Log("Task not completed. Missing required items.");
        }
    }

    public void AssignNextTask()
    {
        foreach (var task in npcTasks)
        {
            if (!task.isCompleted)
            {
                currentTask = task;
                Debug.Log("Assigned new task: " + task.taskName);
                return;
            }
        }

        currentTask = null;
        Debug.Log("No remaining tasks for this NPC.");
    }

    private void RewardPlayer(ItemBaseData rewardItem)
    {
        if (rewardItem != null)
        {
            playerInventory.AddItem(rewardItem);
            Debug.Log("You received: " + rewardItem.itemName);
        }
    }

    public TaskData GetCurrentTask()
    {
        return currentTask;
    }
}