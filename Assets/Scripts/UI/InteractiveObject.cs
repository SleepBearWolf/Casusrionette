using UnityEngine;

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
            }
        }
        else
        {
            PerformStandardInteraction();
        }
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
                    inventorySystem.RemoveItem(requiredItem); // ตรวจสอบว่าถูกลบออกจาก Inventory
                    Debug.Log($"Item {requiredItem.itemName} has been fully used and removed from inventory.");
                }

                if (targetObject != null)
                {
                    Destroy(targetObject); // ตรวจสอบว่า Object ถูกทำลาย
                    Debug.Log($"Destroyed {targetObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Item {requiredItem.itemName} not found in inventory.");
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
