using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Instruments")]
    public List<Instrument> instruments; 
    private List<Instrument> correctOrder; 
    private int currentStep = 0; 

    [Header("Game Objects")]
    public GameObject Wall; 
    public GameObject GameBoard; 

    [Header("Hint Settings")]
    public float hintDelay = 1f; 
    public List<Color> hintColors; 
    public Color errorColor = Color.red; 
    public float blinkDuration = 1f; 

    private bool isShowingHints = false;
    private bool puzzleStarted = false;

    void Awake()
    {
        Instance = this;
        correctOrder = new List<Instrument>(instruments); 
        ShuffleOrder(correctOrder); 
    }

    public bool HasStartedPuzzle()
    {
        return puzzleStarted;
    }

    public bool IsHintSequenceActive()
    {
        return isShowingHints;
    }

    public void CheckInstrument(Instrument instrument)
    {
        if (instrument == correctOrder[currentStep])
        {
            HighlightInstrument(instrument, MakeColorDarker(instrument.defaultColor));

            currentStep++;
            if (currentStep >= correctOrder.Count)
            {
                puzzleStarted = false;
                Destroy(Wall); 
                GameBoard.SetActive(true); 
                Debug.Log("Puzzle Solved!");
            }
        }
        else
        {
            Debug.Log("Incorrect! Resetting...");
            StartCoroutine(ShowErrorAndReset()); 
        }
    }
    private Color MakeColorDarker(Color color)
    {
        float darkFactor = 0.5f; 
        return new Color(color.r * darkFactor, color.g * darkFactor, color.b * darkFactor, color.a);
    }

    public void StartHintSequence()
    {
        if (isShowingHints) return; 
        StartCoroutine(ShowHints());
    }

    private IEnumerator ShowHints()
    {
        isShowingHints = true;

        for (int i = 0; i < correctOrder.Count; i++)
        {
            Instrument instrument = correctOrder[i];
            HighlightInstrument(instrument, hintColors[i % hintColors.Count]);
            instrument.PlaySound();
            yield return new WaitForSeconds(hintDelay); 
            ResetInstrumentAppearance(instrument);
        }

        isShowingHints = false;
        puzzleStarted = true;
        Debug.Log("Hint sequence finished. Puzzle can now be solved.");
    }

    private IEnumerator ShowErrorAndReset()
    {
        foreach (var instrument in instruments)
        {
            HighlightInstrument(instrument, errorColor); 
        }

        yield return new WaitForSeconds(blinkDuration); 

        foreach (var instrument in instruments)
        {
            ResetInstrumentAppearance(instrument);
        }

        ResetPuzzle();

        Debug.Log("Restarting hint sequence after error.");
        StartHintSequence();
    }

    private void HighlightInstrument(Instrument instrument, Color highlightColor)
    {
        Renderer renderer = instrument.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = highlightColor;
        }
    }

    private void ResetInstrumentAppearance(Instrument instrument)
    {
        Renderer renderer = instrument.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = instrument.defaultColor; 
        }
    }

    private void ResetPuzzle()
    {
        currentStep = 0;
        ShuffleOrder(correctOrder); 
        Debug.Log("Puzzle sequence reset.");
    }

    private void ShuffleOrder(List<Instrument> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Instrument temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
