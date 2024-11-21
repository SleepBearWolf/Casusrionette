using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioClip sound; // เสียงของเครื่องดนตรี
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sound;
    }

    void OnMouseDown() // เมื่อมีการคลิกที่เครื่องดนตรี
    {
        PlaySound();
        PuzzleManager.Instance.CheckInstrument(this); // ตรวจสอบว่ากดถูกต้องหรือไม่
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
