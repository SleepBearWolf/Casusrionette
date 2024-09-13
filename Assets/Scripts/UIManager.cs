using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] uiPanels;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject inventoryMenu;
    public float animationDuration = 0.5f; 

    
    public void ShowPanel(GameObject panel)
    {
        foreach (GameObject uiPanel in uiPanels)
        {
            if (uiPanel.activeSelf)
            {
                HidePanel(uiPanel); 
            }
        }

        panel.SetActive(true); 
        LeanTween.moveY(panel.GetComponent<RectTransform>(), 0f, animationDuration).setEase(LeanTweenType.easeOutExpo);
        Debug.Log(panel.name + " is shown.");
    }

    
    public void HidePanel(GameObject panel)
    {
        LeanTween.moveY(panel.GetComponent<RectTransform>(), -Screen.height, animationDuration).setEase(LeanTweenType.easeInExpo).setOnComplete(() =>
        {
            panel.SetActive(false);
        });
        Debug.Log(panel.name + " is hidden.");
    }

    
    public void ShowMainMenu()
    {
        ShowPanel(mainMenu);
    }

    
    public void ShowSettingsMenu()
    {
        ShowPanel(settingsMenu);
    }

    
    public void ShowInventoryMenu()
    {
        ShowPanel(inventoryMenu);
    }

    
    public void ToggleMenu(GameObject panel)
    {
        if (panel.activeSelf)
        {
            HidePanel(panel);
        }
        else
        {
            ShowPanel(panel);
        }
    }

    private void Start()
    {
        foreach (GameObject uiPanel in uiPanels)
        {
            uiPanel.SetActive(false);
        }
        Debug.Log("UIManager has started.");
    }
}
