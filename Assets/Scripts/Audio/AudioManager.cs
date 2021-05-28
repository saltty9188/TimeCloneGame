using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Sound[] music;
    [SerializeField] private Sound[] effects;
    #endregion
    
    #region Public fields
    public static AudioManager instance;
    public const string MASTER_VOLUME_PREF_KEY = "masterVolume";
    public const string MUSIC_VOLUME_PREF_KEY = "musicVolume";
    public const string SFX_VOLUME_PREF_KEY = "SFXVolume";
    #endregion

    #region Private fields
    private Sound currentTrack;
    private Sound targetCountdown;
    private int numPlaying;
    #endregion

    void Awake()
    {  
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in music)
        {
            s.AssignSource(gameObject.AddComponent<AudioSource>());
        }

        foreach(Sound s in effects)
        {
            s.AssignSource(gameObject.AddComponent<AudioSource>());
        }

        targetCountdown = Array.Find<Sound>(effects, sound => sound.Name == "TargetCountdown");
        numPlaying = 0;
    }

    void Start()
    {
        // Set audio levels
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat(MASTER_VOLUME_PREF_KEY, 1));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF_KEY, 1));
        audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 1));
    }

    public void PlaySFX(string name, float pitch = -1)
    {
        Sound sfx = Array.Find<Sound>(effects, sound => sound.Name == name);
        if(sfx == null)
        {
            Debug.LogError("Cannot find sound with name " + name);
            return;
        }

        if(pitch != -1)
        {
            sfx.SetPitch(pitch);
        }

        
        sfx.Play();
    }

    public void PlayMusic(string name)
    {
        Sound track = Array.Find<Sound>(music, sound => sound.Name == name);
        if(track == null)
        {
            Debug.LogError("Cannot find sound with name " + name);
            return;
        }

        // Stop playing the current music if something is playing and it's a different track to the current one
        // TODO: add fade out
        if(currentTrack != null && currentTrack != track)
        {
            currentTrack.Stop();
        }
        if(currentTrack == null || !currentTrack.IsPlaying())
        {
            currentTrack = track;
            currentTrack.Play();
        }
        
    }

    public void PlayLevelTheme(int level)
    {
        if(level > 0 && level <= 3)
        {
            PlayMusic("LevelMusic1");
        } 
        else if(level >= 4 && level <= 5)
        {
            PlayMusic("LevelMusic2");
        }
        else if(level >= 6 && level <= 8)
        {
            PlayMusic("LevelMusic3");
        }
        else if(level >= 9 && level <= 14)
        {
            PlayMusic("LevelMusic4");
        }
    }

    public void PlayTargetCountdown()
    {
        if(!targetCountdown.IsPlaying())
        {
            targetCountdown.Play();
        }
        numPlaying++;
    }

    public void StopTargetCountdown()
    {
        numPlaying--;
        // Only stop when there are no targets left counting down
        if(numPlaying == 0)
        {
            targetCountdown.Stop();
        }
    }
}
