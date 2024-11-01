using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTrap : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(2f, 2f);
    public float captureDuration = 5f;
    private bool isChickenCaptured = false;
    private ChickenAI capturedChicken = null;
    private float captureTimer = 0f;

    private void Update()
    {
        CheckOverlapBox();
        HandleCaptureTimer();
    }

    private void CheckOverlapBox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        bool chickenFound = false;

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState != ChickenAI.ChickenState.Captured)
            {
                chicken.CaptureChicken(gameObject);
                chickenFound = true;
                isChickenCaptured = true;
                capturedChicken = chicken;
                captureTimer = captureDuration;

                ItemPickupAndDraggable itemPickup = chicken.GetComponent<ItemPickupAndDraggable>();
                if (itemPickup != null && itemPickup.RequiresNet)
                {
                    itemPickup.isInNet = true; 
                }

                Debug.Log("Chicken captured for " + captureDuration + " seconds.");
            }
        }

        if (!chickenFound && isChickenCaptured)
        {
            ReleaseChickenFromAll();
            isChickenCaptured = false;
        }
    }

    private void HandleCaptureTimer()
    {
        if (isChickenCaptured && captureTimer > 0f)
        {
            captureTimer -= Time.deltaTime;
        }

        if (captureTimer <= 0f && isChickenCaptured)
        {
            ReleaseChicken();
        }
    }

    private void ReleaseChicken()
    {
        if (capturedChicken != null)
        {
            capturedChicken.ReleaseChicken();
            capturedChicken = null;
            isChickenCaptured = false;

            Vector2 newPosition = new Vector2(transform.position.x + 2f, transform.position.y);
            transform.position = newPosition;

            Debug.Log("Chicken released after timer ended. Net moved.");
        }
    }

    private void ReleaseChickenFromAll()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState == ChickenAI.ChickenState.Captured)
            {
                chicken.ReleaseChicken();
            }
        }
        capturedChicken = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
