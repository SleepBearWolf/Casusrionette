using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public GameObject interactionUI;
    public float interactionDistance = 2f;
    private GameObject player;
    private bool isPlayerNearby = false;
    private bool isMouseOver = false; 

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        interactionUI.SetActive(false);
    }

    void Update()
    {
        CheckInteractionDistance();
    }

    void CheckInteractionDistance()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance || isMouseOver)
        {
            if (!isPlayerNearby && !isMouseOver)
            {
                ShowInteractionUI();
            }

            isPlayerNearby = true;

            if (Input.GetKeyDown(KeyCode.B))
            {
                Interact();
            }
        }
        else
        {
            if (isPlayerNearby && !isMouseOver)
            {
                HideInteractionUI();
            }
            isPlayerNearby = false;
        }
    }

    void ShowInteractionUI()
    {
        interactionUI.SetActive(true);
    }

    void HideInteractionUI()
    {
        interactionUI.SetActive(false);
    }

    void Interact()
    {
        Debug.Log("Player is interacting...");
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        ShowInteractionUI();
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        if (!isPlayerNearby) 
        {
            HideInteractionUI();
        }
    }
}
