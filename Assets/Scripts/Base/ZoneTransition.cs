using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneTransition : MonoBehaviour
{
    public GameObject player;  // ตัวผู้เล่น
    public List<Transform> zoneStartPoints;  // จุดเริ่มต้นของแต่ละโซน
    public float transitionSpeed = 1f;  // ความเร็วของการ Fade

    private int currentZoneIndex = 0;  // โซนปัจจุบันที่ผู้เล่นอยู่
    private bool isTransitioning = false;  // กำลังอยู่ระหว่างการข้ามโซน
    private Image transitionImage;  // ภาพสำหรับ Fade In/Fade Out

    void Start()
    {
        // สร้าง Canvas และ Image สำหรับการ Fade
        CreateFadeCanvas();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isTransitioning)
        {
            StartCoroutine(TransitionToNextZone());
        }
    }

    IEnumerator TransitionToNextZone()
    {
        isTransitioning = true;

        // ทำการ Fade Out (ทำให้จอภาพมืด)
        yield return StartCoroutine(Fade(1));

        // ย้ายผู้เล่นไปยังโซนถัดไป
        currentZoneIndex = (currentZoneIndex + 1) % zoneStartPoints.Count; // วนไปเรื่อย ๆ ตามจำนวนโซน
        player.transform.position = zoneStartPoints[currentZoneIndex].position;

        // ทำการ Fade In (ทำให้จอภาพสว่างขึ้น)
        yield return StartCoroutine(Fade(0));

        isTransitioning = false;
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < transitionSpeed)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / transitionSpeed);
            transitionImage.color = new Color(0, 0, 0, newAlpha);  // เปลี่ยนค่าความโปร่งใส
            yield return null;
        }

        transitionImage.color = new Color(0, 0, 0, targetAlpha);  // ตั้งค่าความโปร่งใสสุดท้าย
    }

    private void CreateFadeCanvas()
    {
        // สร้าง GameObject สำหรับ Canvas
        GameObject canvasObject = new GameObject("TransitionCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);  // กำหนดความละเอียดอ้างอิง

        // สร้าง Image สำหรับทำ Fade
        GameObject imageObject = new GameObject("TransitionImage");
        imageObject.transform.SetParent(canvasObject.transform);
        transitionImage = imageObject.AddComponent<Image>();
        transitionImage.color = new Color(0, 0, 0, 0);  // เริ่มต้นให้โปร่งใส

        // กำหนดขนาดของ Image ให้พอดีกับหน้าจอ
        RectTransform rectTransform = transitionImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
