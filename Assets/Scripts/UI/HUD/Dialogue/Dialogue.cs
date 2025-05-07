using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    public DialogueEnum dialogueEnum;
    public UnityAction onDialogueEnd;
    public enum DialogueEnum
    {
        none,
        mainMenu,
        plushie1,
        plushie2,
        plushie3,
        plushie4,
        plushie5,
        plushie6,
        boss
    }

    public TextMeshProUGUI textComponent;
    private string[] lines;
    public float secsBetweenCharacters;

    private int index;

    public void SetUp(DialogueEnum dialogueEnum)
    {
        this.dialogueEnum = dialogueEnum;

        textComponent.text = string.Empty;
        GetCorrectLines();
        StartDialogue();
    }

    private void GetCorrectLines()
    {
        switch(dialogueEnum)
        {
            case DialogueEnum.mainMenu:
                lines = new string[]
                {
                    "A certain Swedish furniture company has become the largest company in the world, wielding the power of build-it-yourself furniture, incredible meatballs and the cutest of plushies."
                };
                break;
            case DialogueEnum.plushie1:
                lines = new string[]
                {
                    "dwdwdwda da"
                };
                break;
            case DialogueEnum.plushie2:
                lines = new string[]
                {

                };
                break;
            case DialogueEnum.plushie3:
                lines = new string[]
                {

                };
                break;
            case DialogueEnum.plushie4:
                lines = new string[]
                {

                };
                break;
            case DialogueEnum.plushie5:
                lines = new string[]
                {

                };
                break;
            case DialogueEnum.plushie6:
                lines = new string[]
                {

                };
                break;
            case DialogueEnum.boss:
                lines = new string[]
                {

                };
                break;
            default:
                break;
        }
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
        Sound sound = Array.Find(AudioManager.instance.sounds, sound => sound.soundName == AudioManager.SoundEnum.dialogueBeep);
        int beepCooldown = 3;

        //the number of times secsBetweenCharacters passes
        int timePassedSinceBeep = beepCooldown;

        if (index != lines.Length)
        {
            foreach (char character in lines[index].ToCharArray())
            {
                textComponent.text += character;

                timePassedSinceBeep++;
                if (timePassedSinceBeep >= beepCooldown)
                {
                    sound.audioSource.pitch = sound.pitch * UnityEngine.Random.Range(0.7f, 1f);
                    //if(!sound.audioSource.isPlaying) { AudioManager.instance.PlaySound(AudioManager.SoundEnum.dialogueBeep); }
                    AudioManager.instance.PlaySound(AudioManager.SoundEnum.dialogueBeep);
                    timePassedSinceBeep = 0;
                }
                yield return new WaitForSeconds(secsBetweenCharacters);
            }
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
            onDialogueEnd?.Invoke();
            AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
            Destroy(this.gameObject);
        }
    }
}
