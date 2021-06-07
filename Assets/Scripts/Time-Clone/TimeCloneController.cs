using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The TimeCloneController singleton class is responsible for controlling the time-clones and <see cref="TimeCloneDevice">TimeCloneDevices</see> in the level.
/// </summary>
public class TimeCloneController : MonoBehaviour
{
    #region Public fields
    /// <summary>
    /// The single instance of the TimeCloneController.
    /// </summary>
    public static TimeCloneController Instance;
    /// <summary>
    /// List of the time-clones currently active in the level.
    /// </summary>
    public List<GameObject> activeClones;
    #endregion

    #region Inspector fields
    [Tooltip("The text used to display if a clone is out of synch.")]
    [SerializeField] private GameObject _timeCloneText;
    #endregion
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        activeClones = new List<GameObject>();
    }

    /// <summary>
    /// Plays back all of the time-clones recorded in the level except for the one being currently recorded on.
    /// </summary>
    /// <param name="leaveOut">The TimeCloneDevice to not playback.</param>
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

    /// <summary>
    /// Destroys any time-clones that are active in the scene.
    /// </summary>
    /// <param name="destroyProjectiles">Whether or not to also destroy the clone's projectiles</param>
    public void RemoveAllActiveClones(bool destroyProjectiles = true)
    {
        //Remove any previous time-clones
        _timeCloneText.SetActive(false);
        for(int i = 0; i < activeClones.Count; i++)
        {
            GameObject go = activeClones[i];
            // remove the clone from the list if it's already been destroyed
            if(go == null)
            {
                activeClones.Remove(go);
                i--; 
            }
            else if(go.tag == "Clone")
            {
                if(EnemyManager.Targets != null) EnemyManager.Targets.Remove(go);
                if(destroyProjectiles) go.GetComponent<PlayerStatus>().DestroyAllProjectiles(false);
                activeClones.Remove(go);
                go.GetComponent<ExecuteCommands>().RemoveWeapon();
                Destroy(go);
                i--;
            }
        }
    }

    /// <summary>
    /// Removes the <see cref="RecordedCommand">RecordedCommands</see> from all of the <see cref="TimeCloneDevice">TimeCloneDevices</see> in the scene.
    /// </summary>
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

    /// <summary>
    /// Display the out of synch text.
    /// </summary>
    public void OutOfSynch()
    {
        _timeCloneText.SetActive(true);
    }
}
