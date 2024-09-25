using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class SceneLoadPoint
    {
        public enum LoadType { Collision, KeyPress }  
        public LoadType loadType;  

        public GameObject boundaryObject;  
        public KeyCode keyToLoadScene;  
        public string sceneName;  

        public int requiredCollisionCount = 1;  
        public int requiredKeyPressCount = 1;  

        [HideInInspector] public int currentCollisionCount = 0;  
        [HideInInspector] public int currentKeyPressCount = 0;  
    }

    public List<SceneLoadPoint> sceneLoadPoints = new List<SceneLoadPoint>();  
    [SerializeField] private float collisionCooldown = 1f;  
    private bool isInTrigger = false;
    private float lastCollisionTime = 0f;

    private void Update()
    {
        foreach (var loadPoint in sceneLoadPoints)
        {
            if (loadPoint.loadType == SceneLoadPoint.LoadType.KeyPress)
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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var loadPoint in sceneLoadPoints)
        {
            if (loadPoint.loadType == SceneLoadPoint.LoadType.Collision && other.gameObject == loadPoint.boundaryObject && !isInTrigger)
            {
                isInTrigger = true;
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

    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (var loadPoint in sceneLoadPoints)
        {
            if (other.gameObject == loadPoint.boundaryObject)
            {
                isInTrigger = false;
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set for this load point!");
        }
    }
}
