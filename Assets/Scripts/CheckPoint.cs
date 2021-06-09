using UnityEngine;

/// <summary>
/// The CheckPoint class is used to checkpoint the player's progress through a level.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The game object containing all the enemies in the previous room.")]
    [SerializeField] private GameObject _previousEnemyGroup;
    [Tooltip("The door just entered to reach this checkpoint.")]
    [SerializeField] private Door _doorBehind;
    [Tooltip("The new minimum Y position for the camera.")]
    [SerializeField] private float _minCameraY;
    [Tooltip("The new maximum Y position for the camera.")]
    [SerializeField] private float _maxCameraY;
    #endregion

    #region Public fields
    /// <summary>
    /// Static reference to the last checkpoint accessed by the player.
    /// </summary>
    public static CheckPoint lastCheckpoint = null;
    #endregion
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(lastCheckpoint != this && other.tag == "Player") 
        {
            TimeCloneController.Instance.EmptyAllCloneDevices();
            DeleteLastEnemySet();
            SetLastCheckpoint(this);
            if(_doorBehind) _doorBehind.ResetAndTurnOff();
            Recorder r = other.GetComponent<Recorder>();
            if(r) 
            {
                r.CancelRecording();
                r.ResetAllEvents();
            }
            other.GetComponent<PlayerStatus>().SetStartingWeapon();

            Camera.main.GetComponent<CameraTracker>().UpdateMaxAndMin(_minCameraY, _maxCameraY);
        }
    }

    /// <summary>
    /// Sets the last CheckPoint accessed by the player.
    /// </summary>
    /// <param name="newCheckpoint">The CheckPoint accessed.</param>
    static void SetLastCheckpoint(CheckPoint newCheckpoint)
    {
        lastCheckpoint = newCheckpoint;
    }

    void DeleteLastEnemySet()
    {
        if(_previousEnemyGroup != null)
        {
            for(int i = 0; i < _previousEnemyGroup.transform.childCount; i++)
            {
                GameObject enemy = _previousEnemyGroup.transform.GetChild(i).gameObject;
                Destroy(enemy);
            }
        }
    }
}
