using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Target class can activate various <see cref="ButtonEvent">ButtonEvents</see> in the scene when shot.
/// </summary>
public class Target : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The event the target activates.")]
    [SerializeField] private ButtonEvent _attachedEvent;
    #endregion

    #region Protected fields
    /// <value>The ButtonEvent this Target activates.</value>
    protected ButtonEvent AttachedEvent
    {
        get {return _attachedEvent;}
    }
    #endregion

    /// <summary>
    /// Adds an activation to the attached ButtonEvent.
    /// </summary>
    /// <seealso cref="ButtonEvent.AddActivation"/>
    public virtual void Activate()
    {
        if(_attachedEvent) _attachedEvent.AddActivation();
        gameObject.SetActive(false);    
    }

    /// <summary>
    /// Resets the attached ButtonEvent.
    /// </summary>
    /// <seealso cref="ButtonEvent.ResetEvent"/>
    public virtual void ResetTarget()
    {
        gameObject.SetActive(true);
        if(_attachedEvent) _attachedEvent.ResetEvent();
    }
}
