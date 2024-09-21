using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropTool : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    private RectTransform toolRectTransform;
    private Canvas toolCanvas;
    private Vector2 originalToolPosition;
    private bool isToolBeingDragged = false;

    private PlayerItems toolPlayerItems;
    private CanvasGroup toolCanvasGroup;
    private PlayerSystem toolPlayerSystem;

    private void Start()
    {
        toolRectTransform = GetComponent<RectTransform>();
        toolCanvas = GetComponentInParent<Canvas>();
        originalToolPosition = toolRectTransform.anchoredPosition;

        toolPlayerItems = FindObjectOfType<PlayerItems>();
        toolCanvasGroup = GetComponent<CanvasGroup>();
        toolPlayerSystem = FindObjectOfType<PlayerSystem>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (toolPlayerItems.currentTool == null && toolPlayerSystem.IsPointAndClickModeActive())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isToolBeingDragged = true;
                Cursor.visible = false;

                if (toolPlayerItems != null)
                {
                    toolPlayerItems.SetCurrentTool(gameObject);
                }

                if (toolCanvasGroup != null)
                {
                    toolCanvasGroup.blocksRaycasts = false;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            toolPlayerSystem.HideToolUI(); 
        }
    }

    private void Update()
    {
        if (isToolBeingDragged)
        {
            if (toolCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                toolRectTransform.position = Input.mousePosition;
            }
            else if (toolCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 localPointerPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    toolCanvas.transform as RectTransform,
                    Input.mousePosition,
                    toolCanvas.worldCamera,
                    out localPointerPosition);
                toolRectTransform.anchoredPosition = localPointerPosition;
            }
        }
    }

    
    public void ResetToolPosition()
    {
        
        isToolBeingDragged = false;
        Cursor.visible = true;
        toolRectTransform.anchoredPosition = originalToolPosition;

       
        if (toolCanvasGroup != null)
        {
            toolCanvasGroup.blocksRaycasts = true;
        }

        
        if (toolPlayerItems != null)
        {
            toolPlayerItems.SetCurrentTool(null);
        }
    }
}
