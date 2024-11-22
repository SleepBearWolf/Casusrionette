using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public RectTransform inventoryPanel; 
    public Vector3 hiddenPosition; 
    public Vector3 visiblePosition; 
    public float animationDuration = 0.5f; 

    private bool isInventoryOpen = false; 

    private void Start()
    {
        inventoryPanel.localPosition = hiddenPosition;
    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            LeanTween.moveLocal(inventoryPanel.gameObject, hiddenPosition, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            LeanTween.moveLocal(inventoryPanel.gameObject, visiblePosition, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        }

        isInventoryOpen = !isInventoryOpen;
    }

    private void OnDrawGizmosSelected()
    {
        if (inventoryPanel != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.TransformPoint(visiblePosition), 10f); 

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(hiddenPosition), 10f); 
        }
    }
}
