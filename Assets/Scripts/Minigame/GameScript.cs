using UnityEngine;

public class GameScript : MonoBehaviour
{
    [SerializeField] private Transform empthySpace = null;
    private Camera _camera;


    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit)
            {
                if (Vector2.Distance(empthySpace.position, hit.transform.position) < 2)
                {
                    Vector2 lastEmthySpacePosition = empthySpace.position;
                    empthySpace.position = hit.transform.position;
                    hit.transform.position = lastEmthySpacePosition;
                }
            }
        }
        
    }
}
