using System.Collections.Generic;
using UnityEngine;

public class ObjectSequenceManager : MonoBehaviour
{
    [Header("Slots and Objects")]
    public List<TargetSlot> targetSlots;
    public List<DraggableObject> draggableObjects;

    [Header("Success Settings")]
    public GameObject successIndicator;
    public List<GameObject> objectsToActivate;
    public List<GameObject> objectsToDeactivate;

    private void Start()
    {
        if (successIndicator != null)
        {
            successIndicator.SetActive(false);
        }

        foreach (var obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    public void CheckSequence()
    {
        bool isCorrect = true;

        foreach (var slot in targetSlots)
        {
            if (slot.currentObject != slot.correctObject)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            OnSequenceSuccess();
        }
    }

    private void OnSequenceSuccess()
    {
        Debug.Log("Sequence complete!");

        if (successIndicator != null)
        {
            successIndicator.SetActive(true);
        }

        foreach (var obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}
