using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public AudioClip clip;
    public AudioManager.SoundEnum soundName;
    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;
    public AudioMixerGroup mixerGroup;

    [HideInInspector] public AudioSource audioSource;
}

[Serializable]
public class Music
{
    public AudioClip clip;
    public AudioManager.MusicEnum musicName;
    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;
    public AudioMixerGroup mixerGroup;

    [HideInInspector] public AudioSource audioSource;
}
