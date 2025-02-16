using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInput
{
    private List<KeyCode> keySequence = new List<KeyCode>();
    private int currentIndex = 0;
    private TextMeshProUGUI promptText;

    public PlayerInput(KeyCode[] keys)
    {
        foreach (var key in keys)
        {
            keySequence.Add(key); // Add all the keys passed
        }
    }

    public void SetPromptReference(TextMeshProUGUI prompt)
    {
        promptText = prompt; // Reference to the prompt
    }

    public void SetSequence(string sequence)
    {
        keySequence.Clear();
        foreach (char key in sequence)
        {
            if (key == 'W') keySequence.Add(KeyCode.W);
            else if (key == 'A') keySequence.Add(KeyCode.A);
            else if (key == 'S') keySequence.Add(KeyCode.S);
            else if (key == 'D') keySequence.Add(KeyCode.D);
            else if (key == '7') keySequence.Add(KeyCode.Alpha7);
            else if (key == '8') keySequence.Add(KeyCode.Alpha8);
            else if (key == '9') keySequence.Add(KeyCode.Alpha9);
        }
    }

    public bool CheckInput()
    {
        if (currentIndex >= keySequence.Count) return true; // Sequence already completed

        if (Input.GetKeyDown(keySequence[currentIndex]))
        {
            currentIndex++;
        }

        return currentIndex >= keySequence.Count; // Sequence is completed when all keys are pressed
    }

    public void ResetInputState()
    {
        currentIndex = 0; // Reset the input index for the next sequence
    }
}
