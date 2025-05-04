using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup group;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SetVolume);
        slider.value = AudioManager.instance.GetVolume(group);
    }

    public void SetVolume(float linearValue)
    {
        AudioManager.instance.SetVolume(group, linearValue);
    }
}
