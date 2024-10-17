using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool isNearPlayer = false;

    public void StartBounce()
    {
        // Add bouncing or particle effect for drop animation
        GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void Update()
    {
        if (isNearPlayer && Input.GetKeyDown(KeyCode.E))
        {
            CollectItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = true;
            Debug.Log("Player near item");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = false;
            Debug.Log("Player left item");
        }
    }

    void CollectItem()
    {
        // Add the item to the player's inventory (You should implement the actual inventory system)
        Debug.Log(gameObject.name + " collected!");

        // Destroy item after collection
        Destroy(gameObject);
    }
}

