using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private bool destroy = true;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0, clips.Length)];
        source.Play();
    }

    private void Update()
    {
        if (destroy && !source.isPlaying) Destroy(gameObject);
    }
}
