using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public List<Transform> waypoints; 
    public float moveSpeed = 2f; 
    private int currentWaypointIndex = 0; 
    private bool isMoving = false; 

    [Header("Input Settings")]
    public KeyCode interactKey = KeyCode.E; 

    [Header("Trigger Settings")]
    public string triggerTag = "InteractiveObject"; 

    void Update()
    {
        if (isMoving && waypoints.Count > 0)
        {
            MoveTowardsWaypoint();
        }

        if (Input.GetKeyDown(interactKey) && !isMoving)
        {
            MoveToNextWaypoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(triggerTag))
        {
            MoveToNextWaypoint(); 
        }
    }

    private void OnMouseDown()
    {
        if (!isMoving)
        {
            MoveToNextWaypoint();
        }
    }

    public void MoveToNextWaypoint()
    {
        if (!isMoving)
        {
            isMoving = true;
        }
    }

    private void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            isMoving = false; 

            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; 
            }
        }
    }
}
