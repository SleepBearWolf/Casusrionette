using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour
{
    private Vector3 initialPosition;
    private TargetSlot originalSlot;
    private ObjectSequenceManager sequenceManager;
    private bool isDragging = false;

    [Header("Gizmos Settings")]
    public Vector2 gizmosSize = new Vector2(1f, 1f); 

    private void Start()
    {
        initialPosition = transform.position;
        sequenceManager = FindObjectOfType<ObjectSequenceManager>();
    }

    private void OnMouseDown()
    {
        isDragging = true;

        foreach (var slot in sequenceManager.targetSlots)
        {
            if (slot.currentObject == this)
            {
                originalSlot = slot;
                slot.currentObject = null;
                break;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, initialPosition.z);
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, gizmosSize, 0f);

        TargetSlot newSlot = null;

        foreach (var hit in hits)
        {
            TargetSlot slot = hit.GetComponent<TargetSlot>();
            if (slot != null)
            {
                newSlot = slot;
                break;
            }
        }

        if (newSlot != null)
        {
            if (newSlot.currentObject != null && newSlot.currentObject != this)
            {
                DraggableObject swappedObject = newSlot.currentObject;

                if (originalSlot != null)
                {
                    swappedObject.transform.position = originalSlot.transform.position;
                    originalSlot.currentObject = swappedObject;
                }
                else
                {
                    swappedObject.transform.position = swappedObject.initialPosition;
                }

                swappedObject.originalSlot = originalSlot;
            }

            newSlot.currentObject = this;
            originalSlot = newSlot;
            transform.position = newSlot.transform.position;
        }
        else
        {
            if (originalSlot != null)
            {
                transform.position = originalSlot.transform.position;
                originalSlot.currentObject = this;
            }
            else
            {
                transform.position = initialPosition;
            }
        }

        sequenceManager.CheckSequence();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, gizmosSize);
    }
}
