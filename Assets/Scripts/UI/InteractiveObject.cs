using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public float blinkDuration = 0.5f; 
    public float blinkFrequency = 0.1f; 

    [Header("Camera Settings")]
    public CameraTransitionManager cameraManager; 
    public int targetCameraIndex; 

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on this object!");
        }
    }

    private void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void OnMouseDown()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(Blink());
        }

        if (cameraManager != null)
        {
            cameraManager.MoveToCamera(targetCameraIndex);
        }
        else
        {
            Debug.LogWarning("CameraTransitionManager is not assigned!");
        }
    }

    private System.Collections.IEnumerator Blink()
    {
        float elapsedTime = 0f;
        bool toggle = true;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.color = toggle ? highlightColor : originalColor;
            toggle = !toggle;

            elapsedTime += blinkFrequency;
            yield return new WaitForSeconds(blinkFrequency);
        }

        spriteRenderer.color = originalColor;
    }
}
