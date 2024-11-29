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

    private Collider2D[] allColliders;

    private void Start()
    {
        allColliders = FindObjectsOfType<Collider2D>();
    }

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
        pauseMenuUI.SetActive(true);
        SetCollidersActive(false); 
        Debug.Log("Pause Menu Activated");
    }

    public void DeactivateMenu()
    {
        pauseMenuUI.SetActive(false);
        SetCollidersActive(true);
        isPaused = false;
        Debug.Log("Pause Menu Deactivated");
    }

    public void ActivateMenuIn01()
    {
        InMenuUI01.SetActive(true);
        SetCollidersActive(false); 
        Debug.Log("InMenuUI01 Activated");
    }

    public void DeactivateMenuIn01()
    {
        InMenuUI01.SetActive(false);
        SetCollidersActive(true); 
        Debug.Log("InMenuUI01 Deactivated");
    }

    public void ActivateMenuIn02()
    {
        InMenuUI02.SetActive(true);
        SetCollidersActive(false); 
        Debug.Log("InMenuUI02 Activated");
    }

    public void DeactivateMenuIn02()
    {
        InMenuUI02.SetActive(false);
        SetCollidersActive(true); 
        Debug.Log("InMenuUI02 Deactivated");
    }

    public void ActivateMenuIn03()
    {
        InMenuUI03.SetActive(true);
        SetCollidersActive(false); 
        Debug.Log("InMenuUI03 Activated");
    }

    public void DeactivateMenuIn03()
    {
        InMenuUI03.SetActive(false);
        SetCollidersActive(true);
        Debug.Log("InMenuUI03 Deactivated");
    }

    private void SetCollidersActive(bool isActive)
    {
        foreach (var collider in allColliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("UI")) 
            {
                collider.enabled = isActive;
            }
        }
    }
}
