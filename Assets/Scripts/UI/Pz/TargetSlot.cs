using UnityEngine;

public class TargetSlot : MonoBehaviour
{
    public DraggableObject correctObject;
    public DraggableObject currentObject;

    [Header("Gizmos Settings")]
    public Vector2 gizmosSize = new Vector2(1f, 1f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, gizmosSize);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, gizmosSize);
    }
}
