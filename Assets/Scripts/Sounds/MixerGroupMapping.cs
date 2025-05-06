using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class MixerGroupMapping
{
    public AudioMixerGroup group;
    public string parameter;
    public string name => group.name;

    /// <summary>
    /// Sets the linear volume value of <see cref="group"/> to <paramref name="linearVolume"/>
    /// </summary>
    public void Set(float linearVolume)
    {
        group.audioMixer.SetFloat(parameter, Mathf.Log10(PlayerPrefs.GetFloat(name)) * 20);
    }

    /// <returns>Gets the linear volume value of <see cref="group"/></returns>
    /// <exception cref="Exception">If <see cref="parameter"/> is invalid</exception>
    public float Get()
    {
        if (group.audioMixer.GetFloat(parameter, out float volume)) return Mathf.Pow(10, volume / 20);
        else throw new Exception($"Failed to read exported parameter '{parameter}' of '{group.name}'");
    }
}
