using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    private System.Action<bool> onCompleteCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMinigame(System.Action<bool> onComplete)
    {
        // Trigger the minigame popup UI
        Debug.Log("Minigame started");
        onCompleteCallback = onComplete;

        // Simulate minigame (Here you should replace it with actual minigame logic)
        StartCoroutine(SimulateMinigame());
    }

    IEnumerator SimulateMinigame()
    {
        yield return new WaitForSeconds(2f); // Simulating gameplay for 2 seconds

        bool success = Random.Range(0f, 1f) > 0.5f; // 50% chance to pass
        Debug.Log(success ? "Minigame Passed!" : "Minigame Failed!");

        onCompleteCallback?.Invoke(success);
    }
}

