using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The RecordingDoor class is subtype of Door that only opens when the player is recording a time-clone.
/// </summary>
public class RecordingDoor : Door
{
    #region Inspector fields
    [Tooltip("The player's recorder script.")]
    [SerializeField] private Recorder _playerRecorder = null;
    #endregion

    #region Private fields
    private bool _inFight;
    private bool _stayOpen;
    #endregion

    protected override void Start()
    {
        base.Start();
        _inFight = false;
        _stayOpen = false;
    }

    protected override void Update()
    {
        base.Update();
        if(!_playerRecorder.IsRecording) 
        {
            _inFight = false;
        }

        // Open if the player is recording a clone and they aren't in a boss fight
        if(_playerRecorder.IsRecording && !_inFight)
        {
            Activations = RequiredActivations;
        }
        else if(!_stayOpen)
        {
            Activations = 0;
        }  

        TriggerIfValid();
    }

    /// <summary>
    /// Opens the door and keeps it open regardless of whether or not the player is recording.
    /// </summary>
    public void KeepOpen()
    {
        _stayOpen = true;
        Activations = RequiredActivations;
        TriggerIfValid();
    }

    /// <summary>
    /// Sets whether or not the player is in a boss fight.
    /// </summary>
    /// <param name="inFight">Whether the player is in a boss fight or not.</param>
    public void SetInFight(bool inFight)
    {
        this._inFight = inFight;
    }
}
