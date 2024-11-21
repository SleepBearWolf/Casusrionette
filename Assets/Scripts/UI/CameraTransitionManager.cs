using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public List<Camera> cameras = new List<Camera>(); // รายการกล้องทั้งหมด
    public float transitionSpeed = 2f; // ความเร็วของการเปลี่ยนกล้อง

    [Header("Arrow Settings")]
    public List<ArrowSettings> arrowSettings = new List<ArrowSettings>(); // การตั้งค่าลูกศร

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // ระยะเวลาการเฟด
    private Canvas fadeCanvas; // Canvas สำหรับจอดำ
    private Image fadeImage; // Image สีดำใน Canvas

    [Header("Canvas Settings")]
    public List<Canvas> canvases = new List<Canvas>(); // รายการ Canvas ที่ต้องอัปเดต Event Camera

    private int currentCameraIndex = 0; // กล้องปัจจุบัน

    void Start()
    {
        // สร้าง Canvas และ Fade Image
        CreateFadeCanvas();

        // เปิดกล้องเริ่มต้นและปิดกล้องอื่นๆ
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = (i == currentCameraIndex);
        }

        // อัปเดต Event Camera ของ Canvas
        UpdateCanvasCamera();

        // อัปเดตสถานะลูกศร
        UpdateArrows();

        // ปิด Fade Canvas เมื่อไม่ได้ใช้งาน
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(false);
        }
    }

    public void MoveToCamera(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < cameras.Count && targetIndex != currentCameraIndex)
        {
            StartCoroutine(SwitchCamera(targetIndex));
        }
    }

    private IEnumerator SwitchCamera(int newCameraIndex)
    {
        // เปิด Fade Canvas และจอดำ (Fade to Black)
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            yield return StartCoroutine(FadeScreen(true));
        }

        // ปิดกล้องปัจจุบัน
        if (cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].enabled = false;
        }

        // เปลี่ยนไปยังกล้องใหม่
        currentCameraIndex = newCameraIndex;

        if (cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].enabled = true;
        }

        // อัปเดต Event Camera ของ Canvas
        UpdateCanvasCamera();

        // จางจากจอดำกลับมา (Fade to Clear)
        if (fadeCanvas != null)
        {
            yield return StartCoroutine(FadeScreen(false));
            fadeCanvas.gameObject.SetActive(false); // ปิด Fade Canvas หลังจางกลับมา
        }

        // อัปเดตสถานะลูกศร
        UpdateArrows();
    }

    private IEnumerator FadeScreen(bool fadeToBlack)
    {
        float alpha = fadeToBlack ? 0f : 1f;
        float targetAlpha = fadeToBlack ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(alpha, targetAlpha, elapsedTime / fadeDuration);
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, newAlpha); // เปลี่ยนความโปร่งใสของสีดำ
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, targetAlpha); // ตั้งค่าความโปร่งใสสุดท้าย
        }
    }

    private void UpdateArrows()
    {
        foreach (var arrow in arrowSettings)
        {
            if (arrow.arrowObject != null)
            {
                arrow.arrowObject.SetActive(arrow.targetCameraIndex >= 0 && arrow.targetCameraIndex < cameras.Count);
            }
        }
    }

    private void UpdateCanvasCamera()
    {
        foreach (var canvas in canvases)
        {
            if (canvas != null)
            {
                canvas.worldCamera = cameras[currentCameraIndex]; // อัปเดต Event Camera ให้ตรงกับกล้องปัจจุบัน
            }
        }
    }

    private void CreateFadeCanvas()
    {
        // สร้าง Canvas
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // ตั้งค่าเป็น Screen Space - Overlay
        fadeCanvas.sortingOrder = 100; // ให้ Canvas อยู่บนสุด

        // เพิ่ม CanvasScaler
        fadeCanvas.gameObject.AddComponent<CanvasScaler>();

        // เพิ่ม GraphicRaycaster
        fadeCanvas.gameObject.AddComponent<GraphicRaycaster>();

        // สร้าง Image สีดำ
        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false); // ทำให้ Image เป็นลูกของ Canvas
        fadeImage.rectTransform.anchorMin = Vector2.zero; // ยืดเต็มจอ
        fadeImage.rectTransform.anchorMax = Vector2.one;  // ยืดเต็มจอ
        fadeImage.rectTransform.offsetMin = Vector2.zero; // ไม่มี Offset
        fadeImage.rectTransform.offsetMax = Vector2.zero; // ไม่มี Offset
        fadeImage.color = new Color(0, 0, 0, 0); // เริ่มต้นโปร่งใส
    }

    [System.Serializable]
    public class ArrowSettings
    {
        public GameObject arrowObject; // ตัวลูกศรในฉาก
        public int targetCameraIndex; // กล้องเป้าหมายที่ลูกศรนี้จะชี้ไป
    }
}
