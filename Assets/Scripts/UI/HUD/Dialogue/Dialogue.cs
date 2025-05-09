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
                    "A certain Swedish furniture company has become the largest company in the world, wielding the power of build-it-yourself furniture, incredible meatballs and the cutest of plushies.",
                    "The CEO of said furniture company slowly went mad with power named himself the King of BKEA. His power madness became so great that he decided to claim the Title of King of the country.",
                    "In his Madness he decided that children should no longer be happy, deciding to kidnap and torture all of the plushies.",
                    "You, an ex- employee, having seen the horrors of what they are doing to the Plushies so have decided to save them from this.",
                    "To help you achieve this goal you’ve decided to create a competitor to BKEA that sells stolen BKEA products.",
                    "Therefore, every night you go into different BKEAs and take some of their furniture. However, be careful of guards or they might take all your findings.",
                    "In the daytime, you sell this furniture you bought at cheaper prices than original, to attract customers.",
                    "But most importantly: do not forget to try to rescue each Plushie from the madness of the King of BKEA.",
                    "Their fate is in your hands."
                };
                break;
            case DialogueEnum.plushie1:
                lines = new string[]
                {
                    "You have a new message from NALLEBY:\n"
                };
                break;
            case DialogueEnum.plushie2:
                lines = new string[]
                {
                    "You have a new message from SKÖLDIS:\n"
                };
                break;
            case DialogueEnum.plushie3:
                lines = new string[]
                {
                    "You have a new message from ANKLIG:\n"
                };
                break;
            case DialogueEnum.plushie4:
                lines = new string[]
                {
                    "You have a new message from GRODDAN:\n"
                };
                break;
            case DialogueEnum.plushie5:
                lines = new string[]
                {
                    "You have a new message from HUNDRA:\n"
                };
                break;
            case DialogueEnum.plushie6:
                lines = new string[]
                {
                    "You have a new message from BLÅHAJ:\n"
                };
                break;
            case DialogueEnum.boss:
                lines = new string[]
                {
                    //CONTINUE HERE
                    "You have now saved all plushies."
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
