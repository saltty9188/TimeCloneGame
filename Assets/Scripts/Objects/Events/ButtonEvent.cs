using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonEvent : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] protected int requiredActivations = 1;
    #endregion

    #region Public fields
    public bool IsActivated
    {
        get{return activations == requiredActivations;}
    }
    #endregion

    #region Protected fields
    protected int activations;
    #endregion

    #region Private fields
    bool isOn = true;
    #endregion

    //Called by button or other switches to activate the gameObject or event
    public void AddActivation()
    {
        if(isOn)
        {
            activations++;
            TriggerIfValid();
        }
    }

    public void RemoveActivation()
    {
        if(isOn)
        {
            activations--;
            if(activations < 0) activations = 0;
            TriggerIfValid();
        }
    }

    protected void TriggerIfValid()
    {
        if(activations == requiredActivations) Activate();
        else Deactivate();
    }

    public void ResetAndTurnOff()
    {
        isOn = false;
        ResetEvent();
    }

    protected abstract void Activate();

    protected abstract void Deactivate();

    public abstract void ResetEvent();
}