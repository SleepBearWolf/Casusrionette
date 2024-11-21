using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioClip sound; // ���§�ͧ����ͧ�����
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sound;
    }

    void OnMouseDown() // ������ա�ä�ԡ�������ͧ�����
    {
        PlaySound();
        PuzzleManager.Instance.CheckInstrument(this); // ��Ǩ�ͺ��ҡ��١��ͧ�������
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
