using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEntrance : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private RecordingDoor entrance;
    #endregion

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player" || other.tag == "Clone")
        {
            entrance.SetInFight(true);
        }
    }
}
