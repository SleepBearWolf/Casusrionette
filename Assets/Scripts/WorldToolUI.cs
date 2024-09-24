using UnityEngine;
using UnityEngine.UI;

public class WorldToolUI : MonoBehaviour
{
    public string requiredTool; 
    private PlayerItems playerItems; 
    public Button toolButton; 

    private void Start()
    {
        playerItems = FindObjectOfType<PlayerItems>(); 

       
        if (playerItems == null)
        {
            Debug.LogError("PlayerItems ไม่ถูกตั้งค่าใน WorldToolUI!");
        }

        
        if (toolButton != null)
        {
            toolButton.onClick.AddListener(OnButtonClick); 
        }
        else
        {
            Debug.LogError("Button ไม่ถูกตั้งค่าใน WorldToolUI!");
        }
    }

   
    public void OnButtonClick()
    {
        if (playerItems != null && playerItems.HasTool(requiredTool))
        {
            Debug.Log("ใช้เครื่องมือถูกต้อง! ลบ UI");
            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("เครื่องมือที่ใช้ไม่ถูกต้อง! ไม่สามารถลบ UI ได้");
        }
    }
}
