using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class SceneLoadPoint
    {
        public enum LoadTriggerType { Collision, KeyPress }
        public enum LoadActionType { LoadScene, WarpPlayer }

        public LoadTriggerType triggerType;
        public LoadActionType actionType; 

        public GameObject boundaryObject; 
        public KeyCode keyToLoadScene; 
        public string sceneName;
        public Transform warpTarget; 

        public int requiredCollisionCount = 1;
        public int requiredKeyPressCount = 1;
        [HideInInspector] public int currentCollisionCount = 0;
        [HideInInspector] public int currentKeyPressCount = 0;

        public Vector2 boxSize = new Vector2(2f, 2f); 
    }

    public List<SceneLoadPoint> sceneLoadPoints = new List<SceneLoadPoint>();
    private PlayerSystem playerSystem;
    private float lastCollisionTime = 0f;
    [SerializeField] private float collisionCooldown = 1f;
    [SerializeField] private float transitionDuration = 1f;

    private Canvas transitionCanvas;
    private CanvasGroup transitionCanvasGroup;

    private void Start()
    {
        CreateTransitionCanvas();

        playerSystem = FindObjectOfType<PlayerSystem>();
        if (playerSystem == null)
        {
            Debug.LogError("PlayerSystem not found in the scene!");
        }
        else
        {
            Debug.Log("PlayerSystem found: " + playerSystem.gameObject.name);
        }
    }

    private void Update()
    {
        foreach (var loadPoint in sceneLoadPoints)
        {
            bool isPlayerInBox = IsPlayerInBox(loadPoint.boundaryObject, loadPoint.boxSize);

            if (loadPoint.triggerType == SceneLoadPoint.LoadTriggerType.KeyPress && isPlayerInBox)
            {
                if (Input.GetKeyDown(loadPoint.keyToLoadScene))
                {
                    loadPoint.currentKeyPressCount++;
                    Debug.Log("Key press count: " + loadPoint.currentKeyPressCount + " / " + loadPoint.requiredKeyPressCount);

                    if (loadPoint.currentKeyPressCount >= loadPoint.requiredKeyPressCount)
                    {
                        HandleAction(loadPoint);
                        loadPoint.currentKeyPressCount = 0;
                    }
                }
            }

            if (loadPoint.triggerType == SceneLoadPoint.LoadTriggerType.Collision && isPlayerInBox)
            {
                if (Time.time - lastCollisionTime > collisionCooldown)
                {
                    lastCollisionTime = Time.time;
                    loadPoint.currentCollisionCount++;

                    Debug.Log("Collision count: " + loadPoint.currentCollisionCount + " / " + loadPoint.requiredCollisionCount);

                    if (loadPoint.currentCollisionCount >= loadPoint.requiredCollisionCount)
                    {
                        HandleAction(loadPoint);
                        loadPoint.currentCollisionCount = 0;
                    }
                }
            }
        }
    }

    private bool IsPlayerInBox(GameObject boundaryObject, Vector2 boxSize)
    {
        if (playerSystem != null && boundaryObject != null)
        {
            Vector2 boundaryPosition = boundaryObject.transform.position;
            Vector2 playerPosition = playerSystem.transform.position;

            bool isWithinX = Mathf.Abs(playerPosition.x - boundaryPosition.x) <= boxSize.x / 2;
            bool isWithinY = Mathf.Abs(playerPosition.y - boundaryPosition.y) <= boxSize.y / 2;

            return isWithinX && isWithinY;
        }
        return false;
    }

    private void HandleAction(SceneLoadPoint loadPoint)
    {
        if (loadPoint.actionType == SceneLoadPoint.LoadActionType.LoadScene)
        {
            StartCoroutine(LoadSceneWithTransition(loadPoint.sceneName));
        }
        else if (loadPoint.actionType == SceneLoadPoint.LoadActionType.WarpPlayer)
        {
            StartCoroutine(WarpPlayerWithTransition(loadPoint.warpTarget));
        }
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            yield return StartCoroutine(PlayTransition(true));

            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set for this load point!");
        }
    }

    private IEnumerator WarpPlayerWithTransition(Transform target)
    {
        if (target != null)
        {
            yield return StartCoroutine(PlayTransition(true));

            playerSystem.transform.position = target.position;

            yield return StartCoroutine(PlayTransition(false));
        }
        else
        {
            Debug.LogWarning("Warp target is not set for this load point!");
        }
    }

    private IEnumerator PlayTransition(bool isFadingOut)
    {
        float timeElapsed = 0f;
        float startAlpha = isFadingOut ? 0 : 1;
        float endAlpha = isFadingOut ? 1 : 0;

        if (transitionCanvasGroup != null)
        {
            while (timeElapsed < transitionDuration)
            {
                timeElapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / transitionDuration);
                transitionCanvasGroup.alpha = alpha;
                yield return null;
            }
            transitionCanvasGroup.alpha = endAlpha;
        }
    }

    private void CreateTransitionCanvas()
    {
        GameObject canvasObject = new GameObject("TransitionCanvas");
        transitionCanvas = canvasObject.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 1000; 

        transitionCanvasGroup = canvasObject.AddComponent<CanvasGroup>();
        transitionCanvasGroup.alpha = 0; 

        GameObject imageObject = new GameObject("TransitionImage");
        imageObject.transform.SetParent(canvasObject.transform, false);
        Image transitionImage = imageObject.AddComponent<Image>();
        transitionImage.color = Color.black; 

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var loadPoint in sceneLoadPoints)
        {
            if (loadPoint.boundaryObject != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(loadPoint.boundaryObject.transform.position, loadPoint.boxSize);
            }
        }
    }
}
