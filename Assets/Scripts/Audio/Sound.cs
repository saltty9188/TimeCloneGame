using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// The Sound class is a wrapper for assigning AudioClips to AudioSources and performing various actions on them.
/// </summary>
[System.Serializable]
public class Sound
{
    #region Inspector fields
    [Tooltip("The name of this Sound.")]
    [SerializeField] private string _name;
    [Tooltip("The AudioClip associated with this sound.")]
    [SerializeField] private AudioClip _clip;

    [Tooltip("The volume of this Sound.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _volume = 1.0f;

    [Tooltip("The pitch of this Sound.")]
    [Range(0.1f, 3.0f)]
    [SerializeField] private float _pitch = 1.0f;
    [Tooltip("Whether this Sound loops or not.")]
    [SerializeField] private bool _loop;
    [Tooltip("The AudioMixerGroup associated with this Sound.")]
    [SerializeField] private AudioMixerGroup _audioGroup;
    #endregion

    #region Public fields
    /// <value>Gets the name of this Sound.</value>
    public string Name
    {
        get {return _name;}
    }

    /// <value>Gets the current volume of the AudioSource associated with this Sound.</value>
    public float Volume
    {
        get {return _source.volume;}
    }
    #endregion

    #region Private fields
    private AudioSource _source;
    #endregion
    
    /// <summary>
    /// Assigns the AudioSource for this Sound and sets the appropriate fields of the source.
    /// </summary>
    /// <param name="source">The AudioSource this sound will be attached to.</param>
    public void AssignSource(AudioSource source)
    {
        _source = source;
        _source.clip = _clip;
        _source.volume = _volume;
        _source.pitch = _pitch;
        _source.loop = _loop;
        _source.outputAudioMixerGroup = _audioGroup;
    }

    /// <summary>
    /// Sets the pitch for the AudioSource this Sound is associated with.
    /// </summary>
    /// <param name="pitch">The new pitch value.</param>
    public void SetPitch(float pitch)
    {
        _source.pitch = pitch;
    }

    /// <summary>
    /// Resets the pitch of the AudioSource to the one specified by this Sound.
    /// </summary>
    public void ResetPitch()
    {
        _source.pitch = _pitch;
    }

    /// <summary>
    /// Lowers the volume of the AudioSource this Sound is associated with by the given delta value.
    /// </summary>
    /// <param name="delta">The amount the volume is to be lowered by.</param>
    public void LowerVolume(float delta)
    {
        if(_source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return;
        }

        _source.volume -= delta;
    }

    /// <summary>
    /// Resets the volume of the AudioSource to the one specified by this Sound.
    /// </summary>
    public void ResetVolume()
    {
        _source.volume = _volume;
    }

    /// <summary>
    /// Plays the AudioSource this Sound is associated with.
    /// </summary>
    public void Play()
    {
        if(_source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return;
        }

        _source.Play();
    }

    /// <summary>
    /// Stops the AudioSource this Sound is associated with.
    /// </summary>
    public void Stop()
    {
        if(_source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return;
        }
        _source.Stop();
    }

    /// <summary>
    /// Returns true if this Sound is currently playing.
    /// </summary>
    /// <returns>True if this Sound is playing, false otherwise.</returns>
    public bool IsPlaying()
    {
        if(_source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return false;
        }

        return _source.isPlaying;
    }
}
