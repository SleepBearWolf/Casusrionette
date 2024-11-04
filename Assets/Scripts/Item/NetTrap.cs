using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTrap : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(2f, 2f);
    public float captureDuration = 5f;
    private bool isChickenCaptured = false;
    private bool isMonkeyCaptured = false;
    private ChickenAI capturedChicken = null;
    private MonkeyAI capturedMonkey = null;
    private float captureTimer = 0f;

    private void Update()
    {
        CheckOverlapBox();
        HandleCaptureTimer();
    }

    private void CheckOverlapBox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);
        bool entityFound = false;

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState != ChickenAI.ChickenState.Captured)
            {
                chicken.CaptureChicken(gameObject);
                entityFound = true;
                isChickenCaptured = true;
                capturedChicken = chicken;
                captureTimer = captureDuration;

                ItemPickupAndDraggable itemPickup = chicken.GetComponent<ItemPickupAndDraggable>();
                if (itemPickup != null && itemPickup.RequiresNet)
                {
                    itemPickup.SetInNet();
                }

                Debug.Log("Chicken captured for " + captureDuration + " seconds.");
                break;
            }

            MonkeyAI monkey = hit.GetComponent<MonkeyAI>();
            if (monkey != null && monkey.CurrentState != MonkeyAI.MonkeyState.Captured)
            {
                monkey.CaptureMonkey(gameObject);
                entityFound = true;
                isMonkeyCaptured = true;
                capturedMonkey = monkey;
                captureTimer = captureDuration;

                monkey.transform.SetParent(transform); 
                Debug.Log("Monkey captured for " + captureDuration + " seconds.");
                break;
            }
        }

        if (!entityFound && (isChickenCaptured || isMonkeyCaptured))
        {
            ReleaseEntitiesFromAll();
        }
    }

    private void HandleCaptureTimer()
    {
        if ((isChickenCaptured || isMonkeyCaptured) && captureTimer > 0f)
        {
            captureTimer -= Time.deltaTime;
        }

        if (captureTimer <= 0f)
        {
            DestroyNet();
        }
    }

    private void DestroyNet()
    {
        if (isChickenCaptured && capturedChicken != null)
        {
            capturedChicken.ReleaseChicken();
            capturedChicken = null;
        }

        if (isMonkeyCaptured && capturedMonkey != null)
        {
            capturedMonkey.ReleaseMonkey();
            capturedMonkey.transform.SetParent(null); 
            capturedMonkey = null;
        }

        isChickenCaptured = false;
        isMonkeyCaptured = false;
        Destroy(gameObject); 
        Debug.Log("Net destroyed, entities released.");
    }

    private void ReleaseEntitiesFromAll()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState == ChickenAI.ChickenState.Captured)
            {
                chicken.ReleaseChicken();
            }

            MonkeyAI monkey = hit.GetComponent<MonkeyAI>();
            if (monkey != null && monkey.CurrentState == MonkeyAI.MonkeyState.Captured)
            {
                monkey.ReleaseMonkey();
                monkey.transform.SetParent(null); 
            }
        }

        capturedChicken = null;
        capturedMonkey = null;
        isChickenCaptured = false;
        isMonkeyCaptured = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
