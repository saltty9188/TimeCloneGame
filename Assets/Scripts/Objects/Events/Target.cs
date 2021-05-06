using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] protected ButtonEvent attachedEvent;
    #endregion

    public virtual void Activate()
    {
        if(attachedEvent) attachedEvent.AddActivation();
        gameObject.SetActive(false);    
    }

    public void ResetTarget()
    {
        gameObject.SetActive(true);
        if(attachedEvent) attachedEvent.ResetEvent();
    }
}
