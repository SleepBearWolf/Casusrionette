using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitTrap : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(2f, 2f);
    public float lureDuration = 5f;
    private bool isChickenLured = false; 
    private ChickenAI luredChicken = null; 
    private float lureTimer = 0f; 

    private void Update()
    {
        CheckOverlapBox(); 
        HandleLureTimer(); 
    }

    private void CheckOverlapBox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState != ChickenAI.ChickenState.Captured && chicken.CurrentState != ChickenAI.ChickenState.Tired)
            {
                chicken.SetTired(lureDuration); 
                isChickenLured = true;
                luredChicken = chicken;
                lureTimer = lureDuration;

                ItemPickupAndDraggable itemPickup = chicken.GetComponent<ItemPickupAndDraggable>();
                if (itemPickup != null && itemPickup.RequiresNet)
                {
                    itemPickup.SetInNet(); 
                }

                Debug.Log("Chicken lured and tired for " + lureDuration + " seconds.");

                Destroy(gameObject); 
                return;
            }
        }

        if (!isChickenLured)
        {
            ReleaseChickenFromAll(); 
        }
    }

    private void HandleLureTimer()
    {
        if (isChickenLured && lureTimer > 0f)
        {
            lureTimer -= Time.deltaTime;
        }

        if (lureTimer <= 0f && isChickenLured)
        {
            ReleaseChicken(); 
        }
    }

    private void ReleaseChicken()
    {
        if (luredChicken != null)
        {
            luredChicken.CurrentState = ChickenAI.ChickenState.Patrol; 
            luredChicken = null;
            isChickenLured = false;
            Debug.Log("Chicken is no longer tired.");
        }
    }

    private void ReleaseChickenFromAll()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            ChickenAI chicken = hit.GetComponent<ChickenAI>();
            if (chicken != null && chicken.CurrentState == ChickenAI.ChickenState.Tired)
            {
                chicken.CurrentState = ChickenAI.ChickenState.Patrol; 
            }
        }
        luredChicken = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, boxSize); 
    }
}
