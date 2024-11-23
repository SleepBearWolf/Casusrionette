using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReflection : MonoBehaviour
{
    public GameObject lightEffect; 
    private bool isCorrectPosition = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Table")) 
        {
            isCorrectPosition = true;
            ActivateReflection();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            isCorrectPosition = false;
            DeactivateReflection();
        }
    }

    private void ActivateReflection()
    {
        if (lightEffect != null)
        {
            lightEffect.SetActive(true); 
        }
    }

    private void DeactivateReflection()
    {
        if (lightEffect != null)
        {
            lightEffect.SetActive(false); 
        }
    }
}
