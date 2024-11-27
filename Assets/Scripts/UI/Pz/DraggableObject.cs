using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 initialPosition; 
    private TargetSlot originalSlot; 
    private ObjectSequenceManager sequenceManager; 

    private void Start()
    {
        initialPosition = transform.position;
        sequenceManager = FindObjectOfType<ObjectSequenceManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = null;

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

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, initialPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        TargetSlot newSlot = null;

        foreach (var slot in sequenceManager.targetSlots)
        {
            if (Vector3.Distance(transform.position, slot.transform.position) < 0.5f)
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
}
