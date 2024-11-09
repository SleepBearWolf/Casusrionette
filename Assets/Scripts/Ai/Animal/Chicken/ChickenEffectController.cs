using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenEffectController : MonoBehaviour
{
    public GameObject netStunEffectPrefab;       
    public GameObject baitTiredEffectPrefab;     
    public GameObject netDestroyEffectPrefab;    
    public Transform effectPosition;             

    private GameObject currentEffect;           
    private ChickenAI chickenAI;

    private void Awake()
    {
        chickenAI = GetComponent<ChickenAI>();
    }

    private void Update()
    {
        if (chickenAI != null && chickenAI.CurrentState == ChickenAI.ChickenState.Patrol)
        {
            ClearCurrentEffect();
        }
    }

    public void ApplyNetEffect()
    {
        ClearCurrentEffect();

        if (netStunEffectPrefab != null)
        {
            Vector3 spawnPosition = effectPosition != null ? effectPosition.position : transform.position;
            currentEffect = Instantiate(netStunEffectPrefab, spawnPosition, Quaternion.identity, effectPosition != null ? effectPosition : transform);
        }

        if (chickenAI != null)
        {
            chickenAI.CaptureChicken(gameObject);
        }
    }

    public void ApplyBaitEffect(float tiredDuration)
    {
        ClearCurrentEffect();

        if (baitTiredEffectPrefab != null)
        {
            Vector3 spawnPosition = effectPosition != null ? effectPosition.position : transform.position;
            currentEffect = Instantiate(baitTiredEffectPrefab, spawnPosition, Quaternion.identity, effectPosition != null ? effectPosition : transform);
        }

        if (chickenAI != null)
        {
            chickenAI.SetTired(tiredDuration);
        }
    }

    public void TriggerNetDestroyEffect()
    {
        ClearCurrentEffect();

        if (netDestroyEffectPrefab != null)
        {
            Vector3 spawnPosition = effectPosition != null ? effectPosition.position : transform.position;
            Instantiate(netDestroyEffectPrefab, spawnPosition, Quaternion.identity, effectPosition != null ? effectPosition : transform);
        }

        Debug.Log("Net destroy effect triggered.");
    }

    public void ClearCurrentEffect()
    {
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
    }
}
