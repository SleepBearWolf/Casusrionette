using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<TaskData> taskList = new List<TaskData>();
    public PlayerInventory playerInventory;

    public void CheckTaskCompletion(string taskName)
    {
        foreach (TaskData task in taskList)
        {
            if (task.taskName == taskName && task.currentItemCount >= task.requiredItemCount && !task.isCompleted)
            {
                task.isCompleted = true;
                RewardPlayer(task);
                Debug.Log("Task completed: " + task.taskName);
            }
        }
    }

    private void RewardPlayer(TaskData task)
    {
        if (task.rewardItem != null)
        {
            playerInventory.AddItem(task.rewardItem);
            Debug.Log("Player received reward: " + task.rewardItem.itemName);
        }
    }

    public void UpdateTaskProgress(string taskName, int itemCount)
    {
        foreach (TaskData task in taskList)
        {
            if (task.taskName == taskName && !task.isCompleted)
            {
                task.currentItemCount += itemCount;
                Debug.Log("Updated Task: " + task.taskName + " Progress: " + task.currentItemCount + "/" + task.requiredItemCount);
            }
        }
    }

    public bool HasTask(string taskName)
    {
        foreach (TaskData task in taskList)
        {
            if (task.taskName == taskName && !task.isCompleted)
            {
                return true;
            }
        }
        return false;
    }
}
