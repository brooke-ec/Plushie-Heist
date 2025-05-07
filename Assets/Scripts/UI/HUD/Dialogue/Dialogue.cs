using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float secsBetweenCharacters;

    private int index;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (textComponent.text.Equals(lines[index]))
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                //gets current line and fill it out
                textComponent.text = lines[index];
            }
        }
    }
    private void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach(char character in lines[index].ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSeconds(secsBetweenCharacters);
        }
    }

    private void NextLine()
    {
        if(index<lines.Length-1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            print("dialogue done");
        }
    }
}
