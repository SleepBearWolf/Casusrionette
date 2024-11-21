using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public List<Camera> cameras = new List<Camera>(); 
    public float transitionSpeed = 2f; 

    [Header("Arrow Settings")]
    public List<ArrowSettings> arrowSettings = new List<ArrowSettings>();

    [Header("Fade Settings")]
    public float fadeDuration = 1f; 
    private Canvas fadeCanvas; 
    private Image fadeImage;

    [Header("Canvas Settings")]
    public List<Canvas> canvases = new List<Canvas>();

    private int currentCameraIndex = 0; 

    void Start()
    {
        CreateFadeCanvas();

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = (i == currentCameraIndex);
        }

        UpdateCanvasCamera();

        UpdateArrows();

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
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            yield return StartCoroutine(FadeScreen(true));
        }

        if (cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].enabled = false;
        }

        currentCameraIndex = newCameraIndex;

        if (cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].enabled = true;
        }

        UpdateCanvasCamera();

        if (fadeCanvas != null)
        {
            yield return StartCoroutine(FadeScreen(false));
            fadeCanvas.gameObject.SetActive(false); 
        }

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
                fadeImage.color = new Color(0, 0, 0, newAlpha); 
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, targetAlpha); 
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
                canvas.worldCamera = cameras[currentCameraIndex]; 
            }
        }
    }

    private void CreateFadeCanvas()
    {
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay; 
        fadeCanvas.sortingOrder = 100; 

        fadeCanvas.gameObject.AddComponent<CanvasScaler>();

        fadeCanvas.gameObject.AddComponent<GraphicRaycaster>();

        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false); 
        fadeImage.rectTransform.anchorMin = Vector2.zero; 
        fadeImage.rectTransform.anchorMax = Vector2.one;  
        fadeImage.rectTransform.offsetMin = Vector2.zero; 
        fadeImage.rectTransform.offsetMax = Vector2.zero; 
        fadeImage.color = new Color(0, 0, 0, 0); 
    }

    [System.Serializable]
    public class ArrowSettings
    {
        public GameObject arrowObject;
        public int targetCameraIndex;
    }
}
