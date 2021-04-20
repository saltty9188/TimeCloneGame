using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSecondBoss : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private Door doorBehind;

    [SerializeField] private Animator bossAnimator;

    [SerializeField] private GameObject bossUI;
    #endregion
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && bossAnimator)
        {
            doorBehind.ResetEvent();
            bossAnimator.SetTrigger("Start Fight");
            bossAnimator.ResetTrigger("Reset Fight");
            bossUI.SetActive(true);  
        }
    }
}
