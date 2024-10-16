using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<TaskData> taskList = new List<TaskData>();
    public PlayerInventory playerInventory;

    // ตรวจสอบและให้รางวัลเมื่อภารกิจสำเร็จ
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

    // ให้รางวัลแก่ผู้เล่น
    private void RewardPlayer(TaskData task)
    {
        if (task.rewardItem != null)
        {
            playerInventory.AddItem(task.rewardItem);
            Debug.Log("Player received reward: " + task.rewardItem.itemName);
        }
    }

    // อัปเดตความคืบหน้าของภารกิจ
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

    // ตรวจสอบว่ามีภารกิจที่ยังไม่เสร็จหรือไม่
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
