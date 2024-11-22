using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public List<Vector3> waypoints = new List<Vector3>(); 
    public float moveSpeed = 2f; 
    public LeanTweenType moveEaseType = LeanTweenType.linear; 

    private int currentWaypointIndex = 0; 
    private bool isMoving = false; 

    [Header("Debug Gizmos")]
    public bool showGizmos = true; 
    public Color gizmoColor = Color.green; 

    private void OnMouseDown()
    {
        if (!isMoving && waypoints.Count > 0) 
        {
            MoveToNextWaypoint();
        }
    }

    public void AddWaypoint(Vector3 position)
    {
        waypoints.Add(position); 
    }

    public void ClearWaypoints()
    {
        waypoints.Clear(); 
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Count == 0 || isMoving) return;

        isMoving = true;

        Vector3 targetWaypoint = waypoints[currentWaypointIndex];

        LeanTween.move(gameObject, targetWaypoint, moveSpeed)
            .setEase(moveEaseType)
            .setOnComplete(() =>
            {
                isMoving = false;

                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0; 
                }
            });
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || waypoints == null) return;

        Gizmos.color = gizmoColor;

        foreach (var point in waypoints)
        {
            Gizmos.DrawSphere(point, 0.2f);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }
    }
}
