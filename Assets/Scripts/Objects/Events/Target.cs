using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] protected ButtonEvent attachedEvent;
    #endregion

    #region Private fields
    private SpriteRenderer sr;
    #endregion
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public virtual void Activate()
    {
        attachedEvent.AddActivation();
        gameObject.SetActive(false);    
    }

    public void ResetTarget()
    {
        gameObject.SetActive(true);
        attachedEvent.ResetEvent();
    }
}
