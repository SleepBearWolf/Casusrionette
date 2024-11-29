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

    [Header("Collider Settings")]
    [SerializeField] private bool disableCollidersOnPause = true; 
    private Collider2D[] allColliders; 

    [Header("Mouse Settings")]
    [SerializeField] private bool alwaysShowMouse = false; 
    private bool previousMouseVisibility; 

    private void Start()
    {
        allColliders = FindObjectsOfType<Collider2D>();

        Cursor.visible = alwaysShowMouse;
        Cursor.lockState = alwaysShowMouse ? CursorLockMode.None : CursorLockMode.Locked;
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

        if (disableCollidersOnPause)
        {
            SetCollidersActive(false); 
        }

        ManageMouseVisibility(true); 
        Debug.Log("Pause Menu Activated");
    }

    public void DeactivateMenu()
    {
        pauseMenuUI.SetActive(false);

        if (disableCollidersOnPause)
        {
            SetCollidersActive(true);
        }

        ManageMouseVisibility(false); 
        isPaused = false;
        Debug.Log("Pause Menu Deactivated");
    }

    public void ActivateMenuIn01(bool disableColliders)
    {
        InMenuUI01.SetActive(true); 

        if (disableColliders)
        {
            SetCollidersActive(false); 
        }

        ManageMouseVisibility(true); 
        Debug.Log("InMenuUI01 Activated");
    }

    public void DeactivateMenuIn01(bool disableColliders)
    {
        InMenuUI01.SetActive(false); 

        if (disableColliders)
        {
            SetCollidersActive(true); 
        }

        ManageMouseVisibility(false); 
        Debug.Log("InMenuUI01 Deactivated");
    }

    public void ActivateMenuIn02(bool disableColliders)
    {
        InMenuUI02.SetActive(true); 

        if (disableColliders)
        {
            SetCollidersActive(false);
        }

        ManageMouseVisibility(true); 
        Debug.Log("InMenuUI02 Activated");
    }

    public void DeactivateMenuIn02(bool disableColliders)
    {
        InMenuUI02.SetActive(false); 

        if (disableColliders)
        {
            SetCollidersActive(true); 
        }

        ManageMouseVisibility(false); 
        Debug.Log("InMenuUI02 Deactivated");
    }

    public void ActivateMenuIn03(bool disableColliders)
    {
        InMenuUI03.SetActive(true); 

        if (disableColliders)
        {
            SetCollidersActive(false); 
        }

        ManageMouseVisibility(true); 
        Debug.Log("InMenuUI03 Activated");
    }

    public void DeactivateMenuIn03(bool disableColliders)
    {
        InMenuUI03.SetActive(false);

        if (disableColliders)
        {
            SetCollidersActive(true);
        }

        ManageMouseVisibility(false); 
        Debug.Log("InMenuUI03 Deactivated");
    }

    private void SetCollidersActive(bool isActive)
    {
        foreach (var collider in allColliders)
        {
            if (collider != null && collider.gameObject.layer != LayerMask.NameToLayer("UI"))
            {
                Rigidbody2D rigidbody = collider.GetComponent<Rigidbody2D>();
                if (rigidbody == null && !collider.gameObject.CompareTag("Ground"))
                {
                    collider.enabled = isActive; 
                }
            }
        }
    }

    private void ManageMouseVisibility(bool showMouse)
    {
        if (alwaysShowMouse)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = showMouse;
            Cursor.lockState = showMouse ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
