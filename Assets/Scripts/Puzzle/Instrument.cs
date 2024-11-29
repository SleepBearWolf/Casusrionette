using UnityEngine;

public class Instrument : MonoBehaviour
{
    public AudioClip sound; 
    public Color defaultColor = Color.white; 

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sound;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            defaultColor = renderer.material.color; 
        }
    }

    void OnMouseDown()
    {
        if (PuzzleManager.Instance.IsHintSequenceActive())
        {
            Debug.Log("Hint sequence in progress. Wait until it finishes.");
            return;
        }

        if (PuzzleManager.Instance.HasStartedPuzzle())
        {
            PlaySound();
            PuzzleManager.Instance.CheckInstrument(this);
        }
        else
        {
            PuzzleManager.Instance.StartHintSequence();
        }
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
