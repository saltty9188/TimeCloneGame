using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The Recorder class is responsible for recording time-clones.
/// </summary>
public class Recorder : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The prefab for the teleport animation.")]
    [SerializeField] private GameObject _teleportPrefab;
    [Tooltip("The amount of time a recording can go for.")]
    [SerializeField] private float _recordingTimeLimit = 60;
    [Tooltip("The icon that tells the player if they are currently recording or not.")]
    [SerializeField] private GameObject _recordingIcon;
    [Tooltip("Text that displays how long the player has to record.")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [Tooltip("The parent game object holding all of the button events in the scene.")]
    [SerializeField] private GameObject _allEvents;
    [Tooltip("All of the mirror movers in the scene.")]
    [SerializeField] private MirrorMover[] _mirrorMovers;
    #endregion

    #region Public fields
    /// <value>Whether or not the player is currently recording.</value>
    public bool IsRecording
    {
        get{return _recording;}
    }
    #endregion

    #region Private fields
    private Aim _aim;
    private TimeCloneDevice _activeCloneMachine;
    private List<RecordedCommand> _commands;
    private bool _recording;
    private Vector3 _recordingStartPos;
    private Weapon _startingWeapon;
    private float _accumulatedTime;
    private float _timer;
    #endregion

    void Start()
    {
        _aim = transform.GetChild(0).GetComponent<Aim>();
        _recording = false;
        _commands = new List<RecordedCommand>();
        _accumulatedTime = 0;
        _timer = 0;
    }

    void Update()
    {
        if(_timer > 0) 
        {
            _timer -= Time.deltaTime;
            UpdateTimerText();
        }
        else if(_recording)
        {
            _recording = false;
            GetComponent<PlayerController>().RecordingCancelled();
            StopRecording();
        } 
    }

    /// <summary>
    /// Starts the time-clone recording and records the initial status of the player.
    /// </summary>
    /// <remarks>
    /// Also plays back the other time-clones recorded on other <see cref="TimeCloneDevice">TimeCloneDevices</see>.
    /// </remarks>
    /// <param name="nearbyCloneMachine">The TimeCloneDevice where this recording will be stored.</param>
    /// <param name="startingWeapon">The Weapon the player had when they started the recording.</param>
    public void StartRecording(TimeCloneDevice nearbyCloneMachine, Weapon startingWeapon)
    {
        _recording = true;
        // Initialise the starting positions
        PhysicsObject.UpdateAllInitialPositions();
        _recordingStartPos = transform.position;
        _activeCloneMachine = nearbyCloneMachine;
        _activeCloneMachine.SetActiveLight();
        _startingWeapon = startingWeapon;
        foreach(MirrorMover mover in _mirrorMovers)
        {
            mover.SetInitialPositions();
        }
        if(EnemyManager.Instance) EnemyManager.Instance.CacheEnemyInfo();
        if(WeaponManager.Weapons != null) WeaponManager.SetDefaultPosition();

        // Playback previous recordings
        TimeCloneController.Instance.PlayBack(_activeCloneMachine);

        _recordingIcon.SetActive(true);
        _timer = _recordingTimeLimit;
        UpdateTimerText();

        if(_startingWeapon != null)
        {
            if(typeof(PhysicsRay).IsInstanceOfType(startingWeapon))
            {
                PhysicsRay pr = (PhysicsRay) startingWeapon;
                pr.SetStartingType();
            }
        }

        AudioManager.Instance.PlaySFX("StartRecording");
    }

    /// <summary>
    /// Cancels the current recording and does not store it in the current TimeCloneDevice.
    /// </summary>
    /// <remarks>
    /// Can be caused by the player's death or by walking through a new CheckPoint.
    /// </remarks>
    /// <param name="playerDied">Whether or not the cancellation was caused by the player's death.</param>
    public void CancelRecording(bool playerDied = false)
    {
        if(_recording)
        {
            _recording = false;
            _accumulatedTime = 0;
            _commands.Clear();

            _activeCloneMachine.SetEmptyLight();
            _activeCloneMachine = null;
            _recordingIcon.SetActive(false);
            TimeCloneController.Instance.RemoveAllActiveClones();
            GetComponent<PlayerController>().RecordingCancelled();

            // ensure the player is using their default shader
            GetComponent<DamageFlash>().ResetShader();

            // Only reset events if the player died
            if(playerDied)
            {
                ResetAllEvents();
                PhysicsObject.ResetAllPhysics(true, true);
                GetComponent<PlayerStatus>().DestroyAllProjectiles();
                if(EnemyManager.Instance)
                {
                    EnemyManager.Instance.ResetEnemies();
                    EnemyManager.Instance.ResetCurrentBoss();
                }

                if(_aim.CurrentWeapon != null) _aim.DropWeapon(Vector3.zero);
                if(WeaponManager.Weapons != null) WeaponManager.ResetAllWeapons();
                if(_startingWeapon != null) _aim.PickUpWeapon(_startingWeapon);
            }
            else
            {
                foreach(MirrorMover mover in _mirrorMovers)
                {
                    mover.ExitMover();
                }
            }
        }
        foreach(MirrorMover mover in _mirrorMovers)
        {
            mover.ResetPositions();
        }
    }

    /// <summary>
    /// Stops the current recording, stored the recording, and places the player back where they initiated it.
    /// </summary>
    public void StopRecording()
    {
        _recording = false;
        _accumulatedTime = 0;
        transform.parent = null;
        _timer = 0;
        AudioManager.Instance.PlaySFX("EndRecording");

        // store the clone
        _activeCloneMachine.StoreClone(new List<RecordedCommand>(_commands), _recordingStartPos);

        _commands.Clear();
        _activeCloneMachine = null;
        _recordingIcon.SetActive(false);

        // Remove the time-clones from the scene
        TimeCloneController.Instance.RemoveAllActiveClones();

        // Ensure the player is using their default shader
        GetComponent<DamageFlash>().ResetShader();

        if(_aim.CurrentWeapon != null) _aim.DropWeapon(Vector3.zero);
        if(WeaponManager.Weapons != null) WeaponManager.ResetAllWeapons();
        if(_startingWeapon != null) _aim.PickUpWeapon(_startingWeapon);

        StartCoroutine(TeleportAnimation());   
    }

    // teleports the player back to the starting point
    IEnumerator TeleportAnimation()
    {
        // Hide the player
        SetPlayerEnabled(false);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        _aim.gameObject.SetActive(false);

        // Start the teleportation animation
        GameObject tp = Instantiate(_teleportPrefab, transform.position, transform.rotation);
        tp.transform.localScale = transform.localScale;
        //Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = tp.GetComponent<Animator>().runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "TeleportIn")
            {
                animationTime = clip.length;
            }
        }
        yield return new WaitForSeconds(animationTime);

        // Reset the objects and events
        ResetAllEvents();
        PhysicsObject.ResetAllPhysics(true, true);
        GetComponent<PlayerStatus>().DestroyAllProjectiles();
        if(EnemyManager.Instance)
        {
            EnemyManager.Instance.ResetEnemies();
            EnemyManager.Instance.ResetCurrentBoss();
        }

        foreach(MirrorMover mover in _mirrorMovers)
        {
            mover.ResetPositions();
        }

        // Do the second half of the teleport
        transform.position = _recordingStartPos;
        tp.transform.position = _recordingStartPos;
        tp.GetComponent<Animator>().SetTrigger("Reappear");
        //Wait one frame for the animation to start
        yield return null;

        ac = tp.GetComponent<Animator>().runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "TeleportOut")
            {
                animationTime = clip.length;
            }
        }

        yield return new WaitForSeconds(animationTime);

        // Show the player again
        Destroy(tp);
        SetPlayerEnabled(true);
        GetComponent<Rigidbody2D>().gravityScale = 3;
    }

    void SetPlayerEnabled(bool enabled)
    {
        // Can't disable the gameObject so disable the components
        GetComponent<SpriteRenderer>().enabled = enabled;
        GetComponent<PlayerMovement>().enabled = enabled;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = enabled;
    }

    /// <summary>
    /// Resets all of the <see cref="ButtonEvent">ButtonEvents</see> in the level.
    /// </summary>
    /// <seealso cref="ButtonEvent.ResetEvent"/>
    public void ResetAllEvents()
    {
        ResetEvents(_allEvents);
    }

    void ResetEvents(GameObject parent)
    {
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if(child.tag == "Button")
            {
                Button b = child.GetComponent<Button>();
                b.ResetAttachedEvents();
            }
            else if(child.tag == "Target")
            {
                Target t = child.GetComponent<Target>();
                t.ResetTarget();
            }
            else if(child.transform.childCount > 0 && child.GetComponents<Component>().Length == 1)
            {
                ResetEvents(child);
            }
        }
    }

    /// <summary>
    /// Adds a RecordedCommand to the list of commands for the recorded time-clone.
    /// </summary>
    /// <param name="movement">The movement vector for this command.</param>
    /// <param name="jumping">Whether or not the player was holding the jump button for this command.</param>
    /// <param name="angle">The aim angle for the arm for this command.</param>
    /// <param name="shooting">Whether or not the player was holding the shoot button for this command.</param>
    /// <param name="raySwitchValue">The value for switching the current ray type. Positive if next, negative if previous, and 0 if none.</param>
    /// <param name="grabbing">Whether or not the player was holding the grab button for this command.</param>
    /// <param name="movingMirror">Whether or not the player was using a MirrorMover during this command.</param>
    /// <param name="mirrorMoveValue">The value for switching the current object on the MirrorMover. Positive if next, negative if previous, and 0 if none.</param>
    /// <param name="newWeapon">The new weapon picked up by the player on this command. Null if no new weapon was picked up.</param>
    public void AddCommand(Vector2 movement, bool jumping, float angle, bool shooting, float raySwitchValue, bool grabbing, bool movingMirror, float mirrorMoveValue, GameObject newWeapon = null)
    {
        _accumulatedTime += Time.fixedDeltaTime;
        RecordedCommand command = new RecordedCommand(movement, jumping, angle, shooting, _accumulatedTime, raySwitchValue, grabbing, movingMirror, mirrorMoveValue, newWeapon);
        _commands.Add(command);
    }

    void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(_timer / 60.0f);
        float seconds = Mathf.FloorToInt(_timer % 60);
        _timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
