using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject InMenuUI01;
    [SerializeField] private GameObject InMenuUI02;
    [SerializeField] private GameObject InMenuUI03;

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

    public void ActivateMenuIn01()
    {
        Time.timeScale = 1;
        //AudioListener.pause = true;
        InMenuUI01.SetActive(true);
        
        /*Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */
    }

    public void DeactivateMenuIn01()
    {
        Time.timeScale = 1;
        //AudioListener.pause = false;
        InMenuUI01.SetActive(false);
        
        /*Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */
    }

    public void ActivateMenuIn02()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        InMenuUI02.SetActive(true);

        /*Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */
    }

    public void DeactivateMenuIn02()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        InMenuUI02.SetActive(false);

        /*Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */
    }

    public void ActivateMenuIn03()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        InMenuUI03.SetActive(true);

        /*Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */
    }

    public void DeactivateMenuIn03()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        InMenuUI03.SetActive(false);

        /*Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */
    }
}
