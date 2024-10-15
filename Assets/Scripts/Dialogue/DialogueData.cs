using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string dialogueText;
    public bool hasChoices;
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; 
    public DialogueData nextDialogue;  
    public TaskData associatedTask;  

    public bool isTaskRelated()
    {
        return associatedTask != null;
    }
}


