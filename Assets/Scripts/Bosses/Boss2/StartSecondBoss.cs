using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The StartSecondBoss class is a trigger that causes the boss fight to begin.
/// </summary>
public class StartSecondBoss : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The entrance to the boss arena.")]
    [SerializeField] private Door _doorBehind;
    [Tooltip("The animator attached to the boss.")]
    [SerializeField] private Animator _bossAnimator;
    [Tooltip("The GameObject for the boss health bar.")]
    [SerializeField] private GameObject _bossUI;
    #endregion
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && _bossAnimator)
        {
            _doorBehind.ResetEvent();
            _bossAnimator.SetTrigger("Start Fight");
            _bossAnimator.ResetTrigger("Reset Fight");
            _bossUI.SetActive(true);  

            AudioManager.Instance.PlayMusic("BossTheme");
        }
    }
}
