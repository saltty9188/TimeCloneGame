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
    }

    public override void Activate()
    {
        attachedEvent.AddActivation();
        timer = 0;
        SetTargetVisibility(false);
    }

    void Deactivate()
    {
        if(!dontResetWhenActive || (dontResetWhenActive && !attachedEvent.IsActivated))
        {
            SetTargetVisibility(true);
            attachedEvent.RemoveActivation();
        }
    }

    void SetTargetVisibility(bool visible)
    {
        boxCollider2D.enabled = visible;
        spriteRenderer.enabled = visible;
    }
}
