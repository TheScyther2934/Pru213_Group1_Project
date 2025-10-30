using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    public static PlayerDialogue Instance;
    public DialogueBubble dialogueBubble;

    void Awake()
    {
        Instance = this;
        dialogueBubble.Initialize(transform);
    }

    public void ShowDialogue(List<string> messages)
    {
        dialogueBubble.ShowMessages(messages);
    }
}
