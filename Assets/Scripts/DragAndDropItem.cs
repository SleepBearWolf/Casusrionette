using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform originalParent;
    public Canvas canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "Slot")
        {
            transform.SetParent(eventData.pointerEnter.transform);
            transform.position = eventData.pointerEnter.transform.position;
        }
        else
        {

            transform.position = startPosition;
            transform.SetParent(originalParent);
        }
    }
}