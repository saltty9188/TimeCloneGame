using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCloneController : MonoBehaviour
{
    #region Public fields
    public List<GameObject> activeClones;
    #endregion

    void Start()
    {
        activeClones = new List<GameObject>();
    }

    public void PlayBack(TimeCloneDevice leaveOut = null)
    {
        RemoveAllActiveClones();

        for(int i = 0; i < transform.childCount; i++)
        {
            TimeCloneDevice timeCloneDevice = transform.GetChild(i).GetComponent<TimeCloneDevice>();
            if(timeCloneDevice && timeCloneDevice != leaveOut)
            {
                timeCloneDevice.Play();
            }
        }
    }

    public void RemoveAllActiveClones(bool destroyProjectiles = true)
    {
        //Remove any previous time-clones
        for(int i = 0; i < activeClones.Count; i++)
        {
            GameObject go = activeClones[i];
            if(go == null)
            {
                activeClones.Remove(go);
                i--; 
            }
            else if(go.tag == "Clone")
            {
                if(EnemyManager.targets != null) EnemyManager.targets.Remove(go);
                if(destroyProjectiles) go.GetComponent<PlayerStatus>().DestroyAllProjectiles();
                activeClones.Remove(go);
                Destroy(go);
                i--;
            }
        }
    }

    public void EmptyAllCloneDevices()
    {
        RemoveAllActiveClones(false);
        for(int i = 0; i < transform.childCount; i++)
        {
            TimeCloneDevice timeCloneDevice = transform.GetChild(i).GetComponent<TimeCloneDevice>();
            if(timeCloneDevice)
            {
                timeCloneDevice.Empty();
            }
        }
    }
}
