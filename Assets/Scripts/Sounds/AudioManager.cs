using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public Sound[] sounds;
    public Music[] music;
    [SerializeField] private MixerGroupMapping[] persistentGroups;

    public Music currentMusicPlaying { get; private set; }

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

        //currentMusicPlaying = music[0];
    }

    private void Start()
    {
        // Load saved mixer levels
        persistentGroups.ForEach(map =>
        {
            if (PlayerPrefs.HasKey(map.name)) map.Set(PlayerPrefs.GetFloat(map.name));
        });
    }

    private void LateUpdate()
    {
        if (currentMusicPlaying == null || currentMusicPlaying.audioSource != null && !currentMusicPlaying.audioSource.isPlaying)
        {
            //IF SHOP FIND MUSIC FOR THAT
            if (ShopManager.instance != null)
            {
                RandomiseMusicFromType(MusicEnum.levelMusic);
            }
            else if (NightManager.instance != null)
            {
                RandomiseMusicFromType(MusicEnum.nightMusic);
                //OR IF NIGHT FIND MUSIC FOR THAT
            }
            else
            {
                //OTHERWISE PLAY MAIN MENU MUSIC
                PlayMusic(music[0].musicName);
            }
        }
    }

    public void RandomiseMusicFromType(MusicEnum musicType)
    {
        List<Music> musicToChoose = new List<Music>();

        //find all of that type
        for(int i=0; i<music.Length; i++)
        {
            if (music[i]!=null && music[i].musicName.Equals(musicType))
            {
                musicToChoose.Add(music[i]);
            }
        }

        if(musicToChoose.Count > 0)
        {
            int num = UnityEngine.Random.Range(0, musicToChoose.Count);
            PlaySpecificMusic(musicToChoose[num]);
        }
        else
        {
            Debug.LogWarning("No music found for " + musicType);
            PlayMusic(music[0].musicName);
        }
    }

    public void RandomiseSoundFromType(SoundEnum soundType)
    {
        List<Sound> soundToChoose = new List<Sound>();

        //find all of that type
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i] != null && sounds[i].soundName.Equals(soundType))
            {
                soundToChoose.Add(sounds[i]);
            }
        }

        if (soundToChoose.Count > 0)
        {
            int num = UnityEngine.Random.Range(0, soundToChoose.Count);
            PlaySpecificSound(soundToChoose[num]);
        }
        else
        {
            Debug.LogWarning("No sound found for " + soundType);
        }
    }

    /// <summary>
    /// Set and save the linear volume value of <paramref name="group"/>
    /// </summary>
    /// <param name="group">The group to set the volume of</param>
    /// <param name="linearVolume">The volume to set the group to</param>
    /// <exception cref="Exception">If <paramref name="group"/> is not in <see cref="persistentGroups"/></exception>
    public void SetVolume(AudioMixerGroup group, float linearVolume)
    {
        MixerGroupMapping map = persistentGroups.FirstOrDefault(m => m.group == group);
        if (map != null)
        {
            PlayerPrefs.SetFloat(group.name, linearVolume);
            map.Set(linearVolume);
        }
        else throw new Exception($"'{group.name}' is not a persistent group");
    }

    /// <param name="group">The group to get the volume of</param>
    /// <returns>The linear volume value of <paramref name="group"/></returns>
    /// <exception cref="Exception">If <paramref name="group"/> is not in <see cref="persistentGroups"/></exception>
    public float GetVolume(AudioMixerGroup group)
    {
        MixerGroupMapping map = persistentGroups.FirstOrDefault(m => m.group == group);
        if (map != null)
        {
            if (PlayerPrefs.HasKey(group.name)) return PlayerPrefs.GetFloat(group.name);
            return map.Get();
        }
        else throw new Exception($"'{group.name}' is not a persistent group");
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

    public void PlaySpecificSound(Sound soundGiven, int percentageOfVolume = 100)
    {
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
        backpackClose,
        selling,
        dialogueBeep,
        jump,
        ability,
        defeatBoss,
        squeak
    }

    #endregion

    #region Music
    /// <summary>
    /// Play given MusicEnum with a given percentage of volume
    /// </summary>
    public void PlayMusic(MusicEnum musicName, bool immediately = false)
    {
        StopAllCoroutines();

        if (currentMusicPlaying != null)
        {
            if (currentMusicPlaying.musicName == musicName) return;
            if (currentMusicPlaying.musicName != MusicEnum.none)
            {
                if (!immediately)
                {
                    ChangeVolumeGradually(currentMusicPlaying.volume, 0, -0.001f, currentMusicPlaying.audioSource);
                }
                else
                {
                    currentMusicPlaying.audioSource.Stop();
                }
            }
        }

        Music soundGiven = Array.Find(music, sound => sound.musicName == musicName);
        ChangeVolumeGradually(0.05f, soundGiven.volume, 0.001f, soundGiven.audioSource);
        soundGiven.audioSource.Play();

        soundGiven.audioSource.loop = true;
        currentMusicPlaying = soundGiven;
    }

    public void PlaySpecificMusic(Music musicGiven)
    {
        if (currentMusicPlaying != null)
        {
            if (currentMusicPlaying.musicName != MusicEnum.none)
            {
                ChangeVolumeGradually(currentMusicPlaying.volume, 0, -0.001f, currentMusicPlaying.audioSource);
            }
        }
        ChangeVolumeGradually(0.05f, musicGiven.volume, 0.001f, musicGiven.audioSource);
        musicGiven.audioSource.Play();

        musicGiven.audioSource.loop = true;
        currentMusicPlaying = musicGiven;
    }
    public enum MusicEnum
    {
        mainMenu,
        levelMusic,
        nightMusic,
        guardChasingMusic,
        bossFight,
        endOfNight,
        defeatBoss,
        none
    }

    #endregion
}
