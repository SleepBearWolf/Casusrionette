using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropTool : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    private RectTransform rectTransform; 
    private Canvas canvas; 
    private Vector2 originalPosition;
    private bool isDragging = false; 

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>(); 
        canvas = GetComponentInParent<Canvas>(); 
        originalPosition = rectTransform.anchoredPosition; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            Cursor.visible = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Right && isDragging)
        {
            ResetToolPosition();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = Input.mousePosition; 
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 localPointerPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                                                                        Input.mousePosition,
                                                                        canvas.worldCamera,
                                                                        out localPointerPosition);
                rectTransform.anchoredPosition = localPointerPosition; 
            }
        }
    }

   
    public void ResetToolPosition()
    {
        isDragging = false; 
        Cursor.visible = true; 
        rectTransform.anchoredPosition = originalPosition; 
    }
}
