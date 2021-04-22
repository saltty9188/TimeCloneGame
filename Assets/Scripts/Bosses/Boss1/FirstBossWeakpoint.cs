using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossWeakpoint : ButtonEvent
{
    #region Inspector fields
    [SerializeField] private bool activateOnRight = true;
    #endregion

    #region Private fields
    private Animator animator;
    private GameObject weakpoint;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        weakpoint = transform.GetChild(2).gameObject;
    }

    void Update()
    {
        if(animator.GetBool("OnRight") != activateOnRight)
        {
            //Deactivate();
        }
    }

    public override void ResetEvent()
    {
        activations = 0;
        Deactivate();
    }

    protected override void Activate()
    {
        if(animator.GetBool("OnRight") == activateOnRight)
        {
            ExposeWeakPoint();
        }
    }

    protected override void Deactivate()
    {
        HideWeakPoint();
    }
    
    void ExposeWeakPoint()
    {
        if(weakpoint) weakpoint.SetActive(true);
    }

    public void HideWeakPoint()
    {
        if(weakpoint) weakpoint.SetActive(false);
    }
}
