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
        // Handle left click to pick up the tool
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
        // Handle right-click to hide tool menu
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            toolPlayerSystem.HideToolUI(); // ซ่อนเฉพาะ Tool Menu โดยไม่คืนเครื่องมือ
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

    // เพิ่มฟังก์ชัน ResetToolPosition
    public void ResetToolPosition()
    {
        // คืนค่า tool กลับไปที่ตำแหน่งเริ่มต้น
        isToolBeingDragged = false;
        Cursor.visible = true;
        toolRectTransform.anchoredPosition = originalToolPosition;

        // เปิดการตรวจจับ Raycast กลับมา
        if (toolCanvasGroup != null)
        {
            toolCanvasGroup.blocksRaycasts = true;
        }

        // ปล่อยเครื่องมือจาก PlayerItems
        if (toolPlayerItems != null)
        {
            toolPlayerItems.SetCurrentTool(null);
        }
    }
}
