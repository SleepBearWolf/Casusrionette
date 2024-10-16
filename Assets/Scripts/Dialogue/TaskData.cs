using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "TaskSystem/TaskData")]
public class TaskData : ScriptableObject
{
    public string taskName;
    public string description;
    public bool isCompleted;
    public int requiredItemCount;
    public int currentItemCount;
    public string targetNpcName;

    public ItemBaseData rewardItem;  
}
