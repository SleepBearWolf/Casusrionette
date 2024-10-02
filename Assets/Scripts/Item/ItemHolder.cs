using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public ItemBaseData itemData; 
    private bool hasBeenCollected = false;

    public void MarkAsCollected()
    {
        hasBeenCollected = true;
    }

    public void PickUp()
    {
        if (hasBeenCollected)
        {
            Debug.Log("Item has been picked up again: " + itemData.itemName);
        }
    }
}
