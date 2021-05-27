using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    #region Inspector fields
    [SerializeField] private string _name;
    [SerializeField] private AudioClip _clip;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _volume = 1.0f;

    [Range(0.1f, 3.0f)]
    [SerializeField] private float _pitch = 1.0f;
    [SerializeField] private bool _loop;
    [SerializeField] private AudioMixerGroup _audioGroup;
    #endregion

    #region Public fields
    public string Name
    {
        get {return _name;}
    }
    #endregion

    #region Private fields
    private AudioSource source;
    #endregion

    // Assigns the source for this sound and sets the appropriate fields of the source
    public void AssignSource(AudioSource source)
    {
        this.source = source;
        this.source.clip = this._clip;
        this.source.volume = 1;//this._volume;
        this.source.pitch = this._pitch;
        this.source.loop = this._loop;
        this.source.outputAudioMixerGroup = this._audioGroup;
    }

    public void SetPitch(float pitch)
    {
        this._pitch = pitch;
        this.source.pitch = pitch;
    }

    public void Play()
    {
        if(source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return;
        }

        source.Play();
    }

    public void Stop()
    {
        if(source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return;
        }
        source.Stop();
    }

    public bool IsPlaying()
    {
        if(source == null)
        {
            Debug.LogError("AudioSource has not been configured for sound " + _name);
            return false;
        }

        return source.isPlaying;
    }
}
