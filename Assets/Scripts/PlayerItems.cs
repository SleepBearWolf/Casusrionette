using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public List<GameObject> playerTools = new List<GameObject>(); 
    public GameObject currentTool;  

    
    public bool HasTool(string toolName)
    {
        if (currentTool != null && currentTool.name == toolName)
        {
            return true; 
        }
        return false;
    }

    
    public void SetCurrentTool(GameObject tool)
    {
        if (currentTool != null && tool != null)
        {
            Debug.Log("ผู้เล่นมีเครื่องมืออยู่แล้ว: " + currentTool.name);
            return;  
        }

        currentTool = tool; 
        if (tool != null)
        {
            Debug.Log("ถือเครื่องมือใหม่: " + tool.name);
        }
    }

    
    public void AddTool(GameObject newTool)
    {
        playerTools.Add(newTool);
        Debug.Log("เพิ่มเครื่องมือใหม่: " + newTool.name);
    }

    
    public void RemoveCurrentTool()
    {
        if (currentTool != null)
        {
            Debug.Log("ปล่อยเครื่องมือ: " + currentTool.name);
            currentTool = null;  
        }
    }
}
