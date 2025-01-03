﻿using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public float blinkDuration = 0.5f;
    public float blinkFrequency = 0.1f;

    [Header("Camera Settings")]
    public CameraTransitionManager cameraManager;
    public int targetCameraIndex;

    [Header("Transition Settings")]
    public bool useTransition = true;
    public float transitionDelay = 0.5f;

    [Header("Trigger Settings")]
    public string triggerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("Item Usage Settings")]
    public bool requiresItem = false;
    public ItemBaseData requiredItem;
    public GameObject targetObject;

    [Header("Reward Settings")]
    public bool givesRewardItem = false;
    public ItemBaseData rewardItem;

    [Header("Canvas Settings")]
    public bool toggleCanvas = false;
    public GameObject canvasObject;

    [Header("Object Toggle Settings")]
    public bool toggleWithItem = false; 
    public GameObject objectToToggle;   
    public bool toggleState = false;   

    private InventorySystem inventorySystem;
    private bool isPlayerInRange = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on this object!");
        }

        inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found in the scene!");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            HandleInteraction();
        }
    }

    private void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void OnMouseDown()
    {
        if (IsMouseOver())
        {
            HandleInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(triggerTag))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(triggerTag))
        {
            isPlayerInRange = false;
        }
    }

    private bool IsMouseOver()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    private void HandleInteraction()
    {
        if (requiresItem && inventorySystem != null)
        {
            ItemBaseData selectedItem = inventorySystem.GetSelectedItem();
            if (selectedItem != null && selectedItem == requiredItem)
            {
                UseItem();
            }
            else
            {
                Debug.LogWarning("Incorrect item or no item selected!");
                return;
            }
        }

        if (toggleWithItem && objectToToggle != null)
        {
            ToggleObjectWithItem();
        }

        if (toggleCanvas && canvasObject != null)
        {
            ToggleCanvas();
        }

        PerformStandardInteraction();
    }

    private void PerformStandardInteraction()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(Blink());
        }

        if (cameraManager != null)
        {
            if (useTransition)
            {
                StartCoroutine(TransitionWithDelay());
            }
            else
            {
                cameraManager.MoveToCamera(targetCameraIndex);
            }
        }
        else
        {
            Debug.LogWarning("CameraTransitionManager is not assigned!");
        }
    }

    private void UseItem()
    {
        if (requiredItem != null && inventorySystem != null)
        {
            if (inventorySystem.HasItem(requiredItem))
            {
                requiredItem.currentUses++;
                Debug.Log($"Using {requiredItem.itemName}: {requiredItem.currentUses}/{requiredItem.maxUses} uses");

                if (requiredItem.currentUses >= requiredItem.maxUses)
                {
                    inventorySystem.RemoveItem(requiredItem);
                    Debug.Log($"Item {requiredItem.itemName} has been fully used and removed from inventory.");
                }

                if (targetObject != null)
                {
                    Destroy(targetObject);
                    Debug.Log($"Destroyed {targetObject.name}");

                    if (givesRewardItem && rewardItem != null)
                    {
                        GiveRewardItem();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Item {requiredItem.itemName} not found in inventory.");
            }
        }
    }

    private void ToggleObjectWithItem()
    {
        ItemBaseData selectedItem = inventorySystem.GetSelectedItem();
        if (selectedItem != null && selectedItem == requiredItem)
        {
            objectToToggle.SetActive(toggleState);
            Debug.Log($"Object {objectToToggle.name} is now {(toggleState ? "active" : "inactive")}");

            inventorySystem.RemoveItem(selectedItem);
        }
        else
        {
            Debug.LogWarning("Correct item not selected for toggling the object.");
        }
    }

    private void ToggleCanvas()
    {
        bool isActive = canvasObject.activeSelf;
        canvasObject.SetActive(!isActive);
        Debug.Log($"Canvas {canvasObject.name} is now {(isActive ? "hidden" : "shown")}");
    }

    private void GiveRewardItem()
    {
        if (rewardItem != null && inventorySystem != null)
        {
            if (inventorySystem.CanAddItem())
            {
                inventorySystem.AddItem(rewardItem);
                Debug.Log($"Added reward item: {rewardItem.itemName}");
            }
            else
            {
                Debug.LogWarning($"Cannot add reward item: {rewardItem.itemName}. Inventory is full!");
            }
        }
    }

    private System.Collections.IEnumerator Blink()
    {
        float elapsedTime = 0f;
        bool toggle = true;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.color = toggle ? highlightColor : originalColor;
            toggle = !toggle;

            elapsedTime += blinkFrequency;
            yield return new WaitForSeconds(blinkFrequency);
        }

        spriteRenderer.color = originalColor;
    }

    private System.Collections.IEnumerator TransitionWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);

        cameraManager.MoveToCamera(targetCameraIndex);
    }
}
