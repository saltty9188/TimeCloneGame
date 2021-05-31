using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CloseEntrance class is a trigger that closes a given RecordingDoor.
/// </summary>
public class CloseEntrance : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The Door to close.")]
    [SerializeField] private RecordingDoor _entrance;
    #endregion

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player" || other.tag == "Clone")
        {
            _entrance.SetInFight(true);
        }
    }
}
