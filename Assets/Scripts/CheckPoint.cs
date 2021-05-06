using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private TimeCloneController timeCloneController;
    [SerializeField] private GameObject previousEnemyGroup;
    [SerializeField] private Door doorBehind;
    [SerializeField] private float minCameraY;
    [SerializeField] private float maxCameraY;
    #endregion

    #region Public fields
    public static CheckPoint lastCheckpoint = null;
    #endregion
    
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if(lastCheckpoint != this && other.tag == "Player") 
        {
            timeCloneController.EmptyAllCloneDevices();
            DeleteLastEnemySet();
            SetLastCheckpoint(this);
            if(doorBehind) doorBehind.ResetAndTurnOff();
            Recorder r = other.GetComponent<Recorder>();
            if(r) 
            {
                r.CancelRecording();
                r.ResetAllEvents();
            }
            other.GetComponent<PlayerStatus>().SetStartingWeapon();

            Camera.main.GetComponent<CameraTracker>().UpdateMaxAndMin(minCameraY, maxCameraY);
        }
    }

    static void SetLastCheckpoint(CheckPoint newCheckpoint)
    {
        lastCheckpoint = newCheckpoint;
    }

    void DeleteLastEnemySet()
    {
        if(previousEnemyGroup != null)
        {
            for(int i = 0; i < previousEnemyGroup.transform.childCount; i++)
            {
                GameObject enemy = previousEnemyGroup.transform.GetChild(i).gameObject;
                Destroy(enemy);
            }
        }
    }
}
