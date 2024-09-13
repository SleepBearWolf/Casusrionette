using UnityEngine; 
using UnityEngine.UI; 

public class BattleUI : MonoBehaviour
{
    public Text battleStatusText; 

    private void Start()
    {
        
        battleStatusText.text = "Prepare for battle!";
        Debug.Log("BattleUI has started.");
    }

    
    public void UpdateBattleStatus(string status)
    {
        battleStatusText.text = status;
        Debug.Log("Battle status updated: " + status);
    }

    
    public void HideBattleUI()
    {
        gameObject.SetActive(false); 
    }

    
    public void ShowBattleUI()
    {
        gameObject.SetActive(true); 
    }
}
