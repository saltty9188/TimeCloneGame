using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// The AudioManager singleton class is responsible for playing music and sound effects.
/// </summary>
/// <remarks>
/// The class can also fade out the current track.
/// </remarks>
public class AudioManager : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The main audio mixer through which all audio is routed.")]
    [SerializeField] private AudioMixer _audioMixer;
    [Tooltip("An array of Sounds that will be played as music.")]
    [SerializeField] private Sound[] _music;
    [Tooltip("An array of Sounds that will be played as sound effects.")]
    [SerializeField] private Sound[] _effects;
    #endregion
    
    #region Public fields
    /// <summary>
    /// The static instance of this class.
    /// </summary>
    public static AudioManager instance;
    /// <summary>
    /// The PlayerPrefs key for controlling the Master Volume AudioMixer.
    /// </summary>
    public const string MASTER_VOLUME_PREF_KEY = "masterVolume";
    /// <summary>
    /// The PlayerPrefs key for controlling the Music Volume AudioMixerGroup.
    /// </summary>
    public const string MUSIC_VOLUME_PREF_KEY = "musicVolume";
    /// <summary>
    /// The PlayerPrefs key for controlling the Sound Effects Volume AudioMixerGroup.
    /// </summary>
    public const string SFX_VOLUME_PREF_KEY = "SFXVolume";
    /// <summary>
    /// The multiplier for the logarithmic function used to adjust how the audio sliders alter the AudioMixer volume.
    /// </summary>
    public const float AUDIO_MULTIPLIER = 40;
    #endregion

    #region Private fields
    private Sound _currentTrack;
    private Sound _targetCountdown;
    private int _numPlaying;
    #endregion

    void Awake()
    {  
        // Singleton class so destroy this object if its not the instance
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Don't destroy this object when a new scene is loaded
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in _music)
        {
            s.AssignSource(gameObject.AddComponent<AudioSource>());
        }

        foreach(Sound s in _effects)
        {
            s.AssignSource(gameObject.AddComponent<AudioSource>());
        }

        _targetCountdown = Array.Find<Sound>(_effects, sound => sound.Name == "TargetCountdown");
        _numPlaying = 0;

    }

    void Start()
    {
        // Set audio levels
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat(MASTER_VOLUME_PREF_KEY, 1)) * AUDIO_MULTIPLIER);
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF_KEY, 1)) * AUDIO_MULTIPLIER);
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 1)) * AUDIO_MULTIPLIER);
    }

    /// <summary>
    /// Plays the sound effect with the given name, nothing is played if the sound effect does not exist.
    /// </summary>
    /// <param name="name">The name of the sound effect to play.</param>
    /// <param name="pitch">Optionally adjust the pitch of the sound effect to something other than what was specified in 
    /// the sound effects array.</param>
    public void PlaySFX(string name, float pitch = -1)
    {
        // Find the sound effect
        Sound sfx = Array.Find<Sound>(_effects, sound => sound.Name == name);
        if(sfx == null)
        {
            // Log an error if its not found
            Debug.LogError("Cannot find sound with name " + name);
            return;
        }

        // Set the pitch if one was specified
        if(pitch != -1)
        {
            sfx.SetPitch(pitch);
        }
        else
        {
            sfx.ResetPitch();
        }
        sfx.Play();
    }

    /// <summary>
    /// Plays the music with the given name and stops whatever other music track is currently playing, nothing is played if the music track does not exist.
    /// </summary>
    /// <param name="name">The name of the song to play.</param>
    public void PlayMusic(string name)
    {
        // Find the music track
        Sound track = Array.Find<Sound>(_music, sound => sound.Name == name);
        if(track == null)
        {
            Debug.LogError("Cannot find sound with name " + name);
            return;
        }

        // Stop playing the current music if something is playing and it's a different track to the current one
        if(_currentTrack != null && _currentTrack != track)
        {
            _currentTrack.Stop();
        }
        if(_currentTrack == null || !_currentTrack.IsPlaying())
        {
            _currentTrack = track;
            _currentTrack.Play();
        }
    }

    /// <summary>
    /// Plays the theme for the specified level.
    /// </summary>
    /// <param name="level">The number of the current level whose music is to be played.</param>
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

    /// <summary>
    /// Plays the ticking sound effect for the TimedTarget objects.
    /// </summary>
    public void PlayTargetCountdown()
    {
        //Only start playing once
        if(!_targetCountdown.IsPlaying())
        {
            _targetCountdown.Play();
        }
        _numPlaying++;
    }

    /// <summary>
    /// Stops playing the ticking sound effect for the TimedTarget objects.
    /// </summary>
    public void StopTargetCountdown()
    {
        _numPlaying--;
        // Only stop when there are no targets left counting down
        if(_numPlaying <= 0)
        {
            _numPlaying = 0;
            _targetCountdown.Stop();
        }
    }

    ///<summary>Fades out the current song.</summary>
    ///<param name="delta"> The maximum normalised amount for the song to decrease in volume this frame.</param>
    ///<returns>True if the song has faded out completely, false otherwise.</returns>
    public bool FadeOutSong(float delta)
    {
        _currentTrack.LowerVolume(delta);
        Debug.Log(_currentTrack.Volume);
        if(_currentTrack.Volume <= 0)
        {
            _currentTrack.Stop();
            _currentTrack.ResetVolume();
            return true;
        }
        else
        {
            return false;
        }
    }

}
