using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 

    [SerializeField] private bool isPaused = false; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused; 

            if (isPaused)
            {
                ActivateMenu(); 
            }
            else
            {
                DeactivateMenu(); 
            }
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true; 
        pauseMenuUI.SetActive(true); 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; 
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1; 
        AudioListener.pause = false; 
        pauseMenuUI.SetActive(false); 
        isPaused = false; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
