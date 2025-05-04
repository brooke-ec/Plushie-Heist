using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public Sound[] sounds;
    public Music[] music;

    private void Awake()
    {
        if(instance != null) {
            Debug.LogWarning("Another audio manager");
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.outputAudioMixerGroup = sound.mixerGroup;
        }

        foreach (Music sound in music)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.outputAudioMixerGroup = sound.mixerGroup;
        }
    }

    private void Start()
    {
        if (music.Length > 0 && music[0] != null)
        {
            PlayMusic(music[0].musicName);
        }
    }

    public void ChangeVolumeGradually(float startVolume, float targetVolume, float modifier, AudioSource audioSource)
    {
        StartCoroutine(ChangeVolumeCoroutine(startVolume, targetVolume, modifier, audioSource));
    }

    IEnumerator ChangeVolumeCoroutine(float startVolume, float targetVolume, float modifier, AudioSource audioSource)
    {
        audioSource.volume = startVolume;
        if (targetVolume > startVolume)
        {
            while (audioSource.volume < targetVolume)
            {
                audioSource.volume += modifier;
                yield return null;
            }
        }
        else
        {
            while (audioSource.volume > targetVolume)
            {
                audioSource.volume += modifier;
                yield return null;
            }
            audioSource.Stop();
        }
    }


    #region Sound
    /// <summary>
    /// Play given SoundEnum with a given percentage of volume
    /// </summary>
    public void PlaySound(SoundEnum soundName, int percentageOfVolume = 100)
    {
        //Essentially to be able to find the enums
        Sound soundGiven = Array.Find(sounds, sound => sound.soundName == soundName);

        soundGiven.volume = soundGiven.volume * (percentageOfVolume / 100f);
        soundGiven?.audioSource.Play();
    }

    public enum SoundEnum
    {
        UIhover,
        UIclick,
        error,
        bought,
        UIbrowserOpen,
        UIbrowserClose,
        UIclick2,
        unsuccessful,
        successful,
        bell,
        coins,
        lowPitchBell,
        UIclick3,
        backpackOpen,
        backpackClose
    }

    #endregion

    #region Music
    /// <summary>
    /// Play given MusicEnum with a given percentage of volume
    /// </summary>
    public void PlayMusic(MusicEnum musicName)
    {
        Music soundGiven = Array.Find(music, sound => sound.musicName == musicName);
        ChangeVolumeGradually(0.05f, soundGiven.volume, 0.001f, soundGiven.audioSource);
        soundGiven.audioSource.Play();

        soundGiven.audioSource.loop = true;
    }
    public enum MusicEnum
    {
        mainMenu,
        levelMusic,
        none
    }

    #endregion
}
