using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{
    public GameObject toolPanel;  
    private bool isPanelVisible = false;  

    private Vector3 hiddenPosition;  
    private Vector3 shownPosition;   

    public Button toolButton; 

    void Start()
    {
        
        hiddenPosition = toolPanel.transform.localPosition;
        shownPosition = new Vector3(hiddenPosition.x, hiddenPosition.y + 300, hiddenPosition.z);

        
        if (toolButton != null && toolPanel != null)
        {
            toolButton.onClick.AddListener(ToggleToolPanel);
        }
        else
        {
            Debug.LogError("ToolButton or ToolPanel is not connected in Inspector!");
        }
    }

    
    public void ToggleToolPanel()
    {
        if (isPanelVisible)
        {
            LeanTween.moveLocal(toolPanel, hiddenPosition, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            LeanTween.moveLocal(toolPanel, shownPosition, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }

        isPanelVisible = !isPanelVisible;
    }
}
