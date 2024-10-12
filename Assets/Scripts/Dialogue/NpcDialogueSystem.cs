using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcDialogueSystem : MonoBehaviour
{
    public Transform player;
    public Vector2 overlapBoxSize = new Vector2(2f, 2f);
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Elements")]
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;
    public GameObject choiceBox;
    public Button choiceButtonPrefab;

    [Header("Dialogue Data")]
    public DialogueData startingDialogue;
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isTalking = false;
    private bool playerInRange = false;
    private bool isTyping = false;
    private bool showingChoices = false;
    private List<Button> choiceButtons = new List<Button>();
    private Coroutine typingCoroutine;

    [Header("Task System")]
    public NpcTaskSystem taskSystem;   

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private void Start()
    {
        ResetDialogueUI();
    }

    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, overlapBoxSize, 0f);
        playerInRange = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == player)
            {
                playerInRange = true;
                break;
            }
        }

        if (playerInRange && Input.GetKeyDown(interactKey) && !isTalking)
        {
            StartDialogue();
        }

        if (isTalking && Input.GetMouseButtonDown(0) && !isTyping && !showingChoices)
        {
            ShowNextLineOrChoices();
        }

        if (!playerInRange && isTalking)
        {
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        isTalking = true;
        currentLineIndex = 0;
        currentDialogue = startingDialogue;
        dialogueUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterNameText.text = currentDialogue.characterName;
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.dialogueLines[currentLineIndex];

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeDialogue(line.dialogueText));
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void ShowNextLineOrChoices()
    {
        DialogueLine line = currentDialogue.dialogueLines[currentLineIndex];

        if (line.hasChoices)
        {
            ShowChoices(line.choices);
        }
        else
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
    }

    private void ShowChoices(DialogueChoice[] choices)
    {
        choiceBox.SetActive(true);
        showingChoices = true;

        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();

        for (int i = 0; i < choices.Length; i++)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choiceBox.transform);

            TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choices[i].choiceText;
            }

            int choiceIndex = i;

            choiceButton.onClick.AddListener(() => OnChoiceSelected(choices[choiceIndex]));
            choiceButtons.Add(choiceButton);
        }
    }

    private void HideChoices()
    {
        choiceBox.SetActive(false);
        showingChoices = false;

        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        Debug.Log("Choice selected: " + choice.choiceText);

        if (choice.nextDialogue != null)
        {
            currentDialogue = choice.nextDialogue;
            currentLineIndex = 0;
            HideChoices();
            ShowCurrentLine();
        }
        else
        {
            EndDialogue();
        }

        if (taskSystem != null)
        {
            taskSystem.CheckTaskCompletion();  
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        currentLineIndex = 0;
        currentDialogue = startingDialogue;
        dialogueText.text = "";

        HideChoices();

        ResetDialogueUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ResetDialogueUI()
    {
        dialogueUI.SetActive(false);
        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, overlapBoxSize);
    }
}
