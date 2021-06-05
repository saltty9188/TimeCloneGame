using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ButtonEvent class is the base class of objects that are activated by <see cref="Button">Buttons</see> and <see cref="Target">Targets</see>.
/// </summary>
public abstract class ButtonEvent : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("How many buttons are required to activate this event.")]
    [SerializeField] private int _requiredActivations = 1;
    #endregion

    #region Public fields
    /// <value>The number of a buttons that are required to activate this event.</value>
    public int RequiredActivations
    {
        get {return _requiredActivations;}
    }

    /// <value>True if the event is activated, false otherwise.</value>
    public bool IsActivated
    {
        get{return Activations == _requiredActivations;}
    }
    #endregion

    #region Protected fields
    /// <summary>
    /// The number of activations this event currently has.
    /// </summary>
    protected int Activations;
    #endregion

    #region Private fields
    // whether the event can be activated - can be turned off after checkpoint
    private bool _isOn = true;
    #endregion

    /// <summary>
    /// Add's an activation to this event, triggers the event if the threshhold is met.
    /// </summary>
    public void AddActivation()
    {
        if(_isOn)
        {
            Activations++;
            TriggerIfValid();
        }
    }

    /// <summary>
    /// Remove's an activation from this event, deactivates the event.
    /// </summary>
    public void RemoveActivation()
    {
        if(_isOn)
        {
            Activations--;
            if(Activations < 0) Activations = 0;
            TriggerIfValid();
        }
    }

    /// <summary>
    /// Triggers the event if the prerequisite number of activations is met.
    /// </summary>
    protected void TriggerIfValid()
    {
        if(Activations == _requiredActivations) Activate();
        else Deactivate();
    }

    /// <summary>
    /// Deactivates the event and sets it so it cannot be activated again.
    /// </summary>
    public void ResetAndTurnOff()
    {
        _isOn = false;
        ResetEvent();
    }

    /// <summary>
    /// Activates the event.
    /// </summary>
    protected abstract void Activate();

    /// <summary>
    /// Deactivates the event.
    /// </summary>
    protected abstract void Deactivate();

    /// <summary>
    /// Resets the event to its default state
    /// </summary>
    public abstract void ResetEvent();
}