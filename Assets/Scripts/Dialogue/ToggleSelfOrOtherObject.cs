using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSelfOrOtherObject : MonoBehaviour
{
    public GameObject objectToToggle;  
    public float delayBeforeToggle = 3.0f;  
    public bool useSelf = true;  

    public void ToggleOffWithDelay()
    {
        GameObject target = useSelf ? gameObject : objectToToggle; 
        if (target != null && target.activeSelf)  
        {
            StartCoroutine(ToggleAfterDelay(target, false));  
        }
        else
        {
            Debug.Log("Object is already disabled.");
        }
    }

    public void ToggleOnWithDelay()
    {
        GameObject target = useSelf ? gameObject : objectToToggle; 
        if (target != null && !target.activeSelf)  
        {
            StartCoroutine(ToggleAfterDelay(target, true));  
        }
        else
        {
            Debug.Log("Object is already enabled.");
        }
    }

    private IEnumerator ToggleAfterDelay(GameObject target, bool toggleOn)
    {
        yield return new WaitForSeconds(delayBeforeToggle); 

        target.SetActive(toggleOn); 

        Debug.Log("Object has been " + (toggleOn ? "enabled" : "disabled") + " after a delay of " + delayBeforeToggle + " seconds.");
    }

    private void OnDrawGizmos()
    {
        GameObject target = useSelf ? gameObject : objectToToggle;
        Gizmos.color = target != null && target.activeSelf ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
