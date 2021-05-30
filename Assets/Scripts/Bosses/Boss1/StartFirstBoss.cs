using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The StartFirstBoss class is a trigger that causes the boss fight to begin.
/// </summary>
public class StartFirstBoss : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The FirstBossScipt script attached to the boss.")]
    [SerializeField] private FirstBossScript _bossScript;
    [Tooltip("The GameObject for the boss health bar.")]
    [SerializeField] private GameObject _bossUI;
    [Tooltip("The entrance to the boss arena.")]
    [SerializeField] private RecordingDoor _entrance;
    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
        // Start the boss fight and close the entrance if the player has walked in the room
        if(other.tag == "Clone")
        {
            _bossScript.StartFight();
            _bossUI.SetActive(true);
        }
        else if(other.tag == "Player")
        {
            _bossScript.StartFight();
            _bossUI.SetActive(true);
            _entrance.SetInFight(true);

            AudioManager.Instance.PlayMusic("BossTheme");
        }
    }
}
