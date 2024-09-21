using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public List<GameObject> playerTools = new List<GameObject>();  // รายการเครื่องมือทั้งหมดของผู้เล่น
    public GameObject currentTool;  // เครื่องมือที่ผู้เล่นถืออยู่

    // ฟังก์ชันตรวจสอบว่าผู้เล่นถือเครื่องมือที่ถูกต้องหรือไม่
    public bool HasTool(string toolName)
    {
        if (currentTool != null && currentTool.name == toolName)
        {
            return true;  // เครื่องมือที่ผู้เล่นถืออยู่ตรงกับที่ระบุ
        }
        return false;
    }

    // ฟังก์ชันสำหรับตั้งเครื่องมือที่ผู้เล่นถือ
    public void SetCurrentTool(GameObject tool)
    {
        if (currentTool != null && tool != null)
        {
            Debug.Log("ผู้เล่นมีเครื่องมืออยู่แล้ว: " + currentTool.name);
            return;  // ไม่สามารถถือเครื่องมือซ้ำได้จนกว่าจะปล่อยเครื่องมือเดิม
        }

        currentTool = tool;  // ตั้งค่าเครื่องมือใหม่
        if (tool != null)
        {
            Debug.Log("ถือเครื่องมือใหม่: " + tool.name);
        }
    }

    // ฟังก์ชันสำหรับเพิ่มเครื่องมือในรายการ
    public void AddTool(GameObject newTool)
    {
        playerTools.Add(newTool);
        Debug.Log("เพิ่มเครื่องมือใหม่: " + newTool.name);
    }

    // ฟังก์ชันสำหรับปล่อยเครื่องมือปัจจุบัน
    public void RemoveCurrentTool()
    {
        if (currentTool != null)
        {
            Debug.Log("ปล่อยเครื่องมือ: " + currentTool.name);
            currentTool = null;  // ปล่อยเครื่องมือที่ถืออยู่
        }
    }
}
