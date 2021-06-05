using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FirstBossWeakpoint class is a ButtonEvent that shows the first boss's weakpoint when the appropriate Button is pressed.
/// </summary>
public class FirstBossWeakpoint : ButtonEvent
{
    #region Inspector fields
    [Tooltip("Will this script activate when the boss is on the right or the left?")]
    [SerializeField] private bool _activateOnRight = true;
    #endregion

    #region Private fields
    private Animator _animator;
    private GameObject _weakpoint;
    #endregion

    void Start()
    {
        _animator = GetComponent<Animator>();
        _weakpoint = transform.GetChild(2).gameObject;
    }

    /// <summary>
    /// Resets the number of activations and hides the weakpoint.
    /// </summary>
    public override void ResetEvent()
    {
        Activations = 0;
        Deactivate();
    }

    /// <summary>
    /// Exposes the weakpoint only if the boss is currently on the correct side of the arena.
    /// </summary>
    protected override void Activate()
    {
        if(_animator.GetBool("OnRight") == _activateOnRight)
        {
            ExposeWeakPoint();
        }
    }

    /// <summary>
    /// Hides the boss weakpoint.
    /// </summary>
    protected override void Deactivate()
    {
        HideWeakPoint();
    }
    
    void ExposeWeakPoint()
    {
        if(_weakpoint) _weakpoint.SetActive(true);
    }

    /// <summary>
    /// Public function for directly hiding the boss weakpoint.
    /// </summary>
    public void HideWeakPoint()
    {
        if(_weakpoint) _weakpoint.SetActive(false);
    }
}
