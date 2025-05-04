using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string mixerGroupName;

    public void SetVolume(float sliderValue)
    {
        //converts slider value to log value of base 10
        audioMixer.SetFloat(mixerGroupName, Mathf.Log10(sliderValue) * 20);
    }
}
