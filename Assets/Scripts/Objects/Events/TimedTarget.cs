using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTarget : Target
{
    #region Inspector fields
    [SerializeField] private float resetTime = 5;
    [SerializeField] private bool dontResetWhenActive;
    #endregion

    #region Private fields
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private float timer;
    #endregion

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = resetTime;
    }

    void Update()
    {
        if(timer < resetTime) timer += Time.deltaTime;
        else if(!boxCollider2D.enabled) Deactivate();

        if(dontResetWhenActive && attachedEvent.IsActivated)
        {
            AudioManager.Instance.StopTargetCountdown();
        }
    }

    public override void Activate()
    {
        attachedEvent.AddActivation();
        timer = 0;
        SetTargetVisibility(false);
        AudioManager.Instance.PlayTargetCountdown();
    }

    void Deactivate()
    {
        // Only deactivate if the don't reset when active flag is set
        // or it is set and the attached event is not active (i.e. not all targets were hit in time)
        if(!dontResetWhenActive || (dontResetWhenActive && !attachedEvent.IsActivated))
        {
            AudioManager.Instance.StopTargetCountdown();
            SetTargetVisibility(true);
            attachedEvent.RemoveActivation();
        }
    }

    public override void ResetTarget()
    {
        base.ResetTarget();
        AudioManager.Instance.StopTargetCountdown();
    }

    void SetTargetVisibility(bool visible)
    {
        boxCollider2D.enabled = visible;
        spriteRenderer.enabled = visible;
    }
}
