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
                    "You have a new message from NALLEBY:\n",
                    "Thank you for saving me from the evil king, He is a truly terrible man.",
                    "But it's not just me please save my friends as well!",
                    "While I'm here though, I can help out a bit:",
                    "Check out your Skill trees and you should see some cool new stuff you can unlock you help you out, including a very cool double jump."
                };
                break;
            case DialogueEnum.plushie2:
                lines = new string[]
                {
                    "You have a new message from SKÖLDIS:\n",
                    "My friend, Thank you so much for helping me out of there, That evil king did some terrible things to me...",
                    "I'm afraid he may be doing similar things to my friends, please go and save them!!",
                    "As a thank you for help I've added some new stuff to your skill tree including the very cool new Boost Skill."
                };
                break;
            case DialogueEnum.plushie3:
                lines = new string[]
                {
                    "You have a new message from ANKLIG:\n",
                    "You are a Legend for getting me out of there Dude!!",
                    "That Evil King dude was not a vibe at all man!",
                    "Ayo tho, some of my friends are still stuck in that hellhole, you gotta get them out my guy!!!",
                    "Btw I helped get you some cool new stuff, I recommend getting that wall running stuff, it's Super Awesome Dude!!!!"
                };
                break;
            case DialogueEnum.plushie4:
                lines = new string[]
                {
                    "You have a new message from GRODDAN:\n",
                    "Hey, thanks for saving, I don't know what I would have done without you.",
                    "There's still a couple of my friends left in there and I was wondering if you could maybe help them out like you did with me.",
                    "I also added some cool new stuff on the skill tree for you as a thank you, that Automatic Restock is a real treat."
                };
                break;
            case DialogueEnum.plushie5:
                lines = new string[]
                {
                    "You have a new message from HUNDRA:\n",
                    "Thanks so much for saving me you are the best!! That King is super evil and I don't know how much longer I would have lasted.",
                    "My friend the BLÅHAJ is still in there, please go save them!",
                    "Oh, you should also check out the skill tree, the new grapple and glide a well worth unlocking."

                };
                break;
            case DialogueEnum.plushie6:
                lines = new string[]
                {
                    "You have a new message from BLÅHAJ:\n",
                    "My dearest thanks for saving me and my friends from that terrible King.",
                    "I don't know what we would have done without you, you truly are a real hero.",
                    "I have added the power to escape guards to your skill tree if you insist on going into that horrible place again, ",
                    "but be careful with it, it can only help so much.",
                    "From now on, you can keep building your furniture empire every day and every night, but there will be no more of us to rescue.",
                    "Thank you for everything.",
                    "You have now saved all plushies."
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
