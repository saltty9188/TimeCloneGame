using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The StartThirdBoss class is a trigger that causes the boss fight to begin.
/// </summary>
public class StartThirdBoss : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The main script for the third boss fight.")]
    [SerializeField] private ThirdBossScript _bossScript;
    [Tooltip("The GameObject that holds the boss UI.")]
    [SerializeField] private GameObject _bossUI;
    [Tooltip("The main entrance to the boss arena.")]
    [SerializeField] private RecordingDoor _entrance;
    #endregion

    void OnTriggerEnter2D(Collider2D other) 
    {
        // Start the fight when triggered
        if(other.tag == "Clone")
        {
            _bossScript.StartFight();
            _bossUI.SetActive(true);
        }
        // Only close the entrance and start the boss fight if the player passes through the trigger
        else if(other.tag == "Player")
        {
            _bossScript.StartFight();
            _bossUI.SetActive(true);
            _entrance.SetInFight(true);

            AudioManager.Instance.PlayMusic("FinalBossTheme");
        }
    }
}
