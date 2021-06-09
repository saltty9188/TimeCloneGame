using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The TimedTarget class is a variant of Target that resets itself on a timer.
/// </summary>
public class TimedTarget : Target
{
    #region Inspector fields
    [Tooltip("How long it takes for the target to reset itself.")]
    [SerializeField] private float _resetTime = 5;
    [Tooltip("Should the timer stop when the attached event is active?")]
    [SerializeField] private bool _dontResetWhenActive;
    #endregion

    #region Private fields
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    private float _timer;
    #endregion

    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _timer = _resetTime;
    }

    void Update()
    {
        if(_timer < _resetTime) _timer += Time.deltaTime;
        else if(!_boxCollider2D.enabled) Deactivate();

        if(_dontResetWhenActive && AttachedEvent.IsActivated)
        {
            AudioManager.Instance.StopTargetCountdown();
        }
    }

    /// <summary>
    /// Adds an activation to the attached ButtonEvent and starts the timer.
    /// </summary>
    /// <seealso cref="ButtonEvent.AddActivation"/>    
    public override void Activate()
    {
        AttachedEvent.AddActivation();
        _timer = 0;
        SetTargetVisibility(false);
        AudioManager.Instance.PlayTargetCountdown();
    }

    void Deactivate()
    {
        // Only deactivate if the don't reset when active flag is set
        // or it is set and the attached event is not active (i.e. not all targets were hit in time)
        if(!_dontResetWhenActive || (_dontResetWhenActive && !AttachedEvent.IsActivated))
        {
            AudioManager.Instance.StopTargetCountdown();
            SetTargetVisibility(true);
            AttachedEvent.RemoveActivation();
        }
    }

    public override void ResetTarget()
    {
        base.ResetTarget();
        AudioManager.Instance.StopTargetCountdown();
    }

    void SetTargetVisibility(bool visible)
    {
        _boxCollider2D.enabled = visible;
        _spriteRenderer.enabled = visible;
    }
}
