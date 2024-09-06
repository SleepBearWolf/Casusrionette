using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    private GameObject heldObject = null; 
    private Vector3 mouseOffset; 
    private float mouseZCoord; 

    private bool is3DMode; 

    void Update()
    {
        
        HandlePointAndClickMode();

        
        if (heldObject != null)
        {
            DragObject();
        }
    }

    private void HandlePointAndClickMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                if (is3DMode)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("PickUpObject"))
                    {
                        PickUpObject(hit.collider.gameObject);
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null && hit.collider.CompareTag("PickUpObject"))
                    {
                        PickUpObject(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                DropObject();
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        heldObject = obj;

        if (is3DMode)
        {
            mouseZCoord = Camera.main.WorldToScreenPoint(heldObject.transform.position).z;
            mouseOffset = heldObject.transform.position - GetMouseWorldPosition3D();
            obj.GetComponent<Rigidbody>().isKinematic = true; 
        }
        else
        {
            mouseOffset = heldObject.transform.position - GetMouseWorldPosition2D();
            obj.GetComponent<Rigidbody2D>().isKinematic = true; 
        }

        Debug.Log("Picked up: " + obj.name);
    }

    private void DropObject()
    {
        if (is3DMode)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; 
        }
        else
        {
            heldObject.GetComponent<Rigidbody2D>().isKinematic = false; 
        }

        heldObject = null;
        Debug.Log("Dropped object");
    }

    private void DragObject()
    {
        if (is3DMode)
        {
            heldObject.transform.position = GetMouseWorldPosition3D() + mouseOffset;
        }
        else
        {
            heldObject.transform.position = GetMouseWorldPosition2D() + mouseOffset;
        }
    }

    private Vector3 GetMouseWorldPosition2D()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 GetMouseWorldPosition3D()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void SetMode(bool is3D)
    {
        is3DMode = is3D;
    }
}
