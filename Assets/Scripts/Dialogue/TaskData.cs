using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Task", menuName = "Task/TaskData")]
public class TaskData : ScriptableObject
{
    public string taskName;           
    public string taskDescription;   
    public ItemBaseData requiredItem; 
    public int requiredAmount = 1;    
    public bool isCompleted = false;  
    public ItemBaseData rewardItem;   
}
