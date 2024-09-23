using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropTool : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private bool isFollowingMouse = false;

    private PlayerItems playerItems;
    private CanvasGroup canvasGroup;
    private PlayerSystem playerSystem;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;

        playerItems = FindObjectOfType<PlayerItems>();
        canvasGroup = GetComponent<CanvasGroup>();
        playerSystem = FindObjectOfType<PlayerSystem>();

        if (canvasGroup != null)
        {
            
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (playerItems.currentTool == null && playerSystem.IsPointAndClickModeActive())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                isFollowingMouse = true;
                Cursor.visible = false;

                if (playerItems != null)
                {
                    playerItems.SetCurrentTool(gameObject);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = false;  
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Right && isFollowingMouse)
        {
            CancelTool();  
        }
    }

    
    public void CancelTool()
    {
        isFollowingMouse = false;
        rectTransform.anchoredPosition = originalPosition;

        
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;  
        }

        
        if (playerItems != null)
        {
            playerItems.RemoveCurrentTool();
        }

        Cursor.visible = true;  
    }

    public void ResetToolPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        isFollowingMouse = false;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void Update()
    {
        
        if (isFollowingMouse)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = Input.mousePosition;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 localPointerPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    Input.mousePosition,
                    canvas.worldCamera,
                    out localPointerPosition);
                rectTransform.anchoredPosition = localPointerPosition;
            }
        }

        
        if (Input.GetKeyDown(KeyCode.E) && isFollowingMouse)
        {
            CancelTool();  
        }
    }
}
