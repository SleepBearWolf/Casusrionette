using System.Collections.Generic;
using UnityEngine;

public class TaskDatabase : MonoBehaviour
{
    public static TaskDatabase Instance;

    public List<TaskData> allTasks = new List<TaskData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TaskData GetTask(string taskName)
    {
        return allTasks.Find(task => task.taskName == taskName);
    }
}
