using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class SceneLoadPoint
    {
        public enum LoadTriggerType { Collision, KeyPress }
        public LoadTriggerType triggerType;

        public GameObject boundaryObject;
        public KeyCode keyToLoadScene;
        public string sceneName; 

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

    private void Start()
    {

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
                        LoadScene(loadPoint.sceneName);
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
                        LoadScene(loadPoint.sceneName);
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

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set for this load point!");
        }
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
