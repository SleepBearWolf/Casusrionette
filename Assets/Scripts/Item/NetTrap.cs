﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTrap : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(2f, 2f);
    public float captureDuration = 5f;
    public GameObject netReleaseEffectPrefab; 
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
                    itemPickup.SetInNet();
                }

                Debug.Log("Chicken captured for " + captureDuration + " seconds.");
                break;
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
            ReleaseChickenWithEffect();
        }
    }

    private void ReleaseChickenWithEffect()
    {
        if (capturedChicken != null)
        {
            TriggerNetReleaseEffect();

            capturedChicken.ReleaseChicken();
            capturedChicken = null;
            isChickenCaptured = false;

            Debug.Log("Chicken released with net destroy effect after timer ended.");
        }
    }

    private void TriggerNetReleaseEffect()
    {
        if (netReleaseEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            Instantiate(netReleaseEffectPrefab, spawnPosition, Quaternion.identity);
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
