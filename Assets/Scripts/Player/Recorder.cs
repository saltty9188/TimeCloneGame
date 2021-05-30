using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Recorder : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private GameObject teleportPrefab;
    [SerializeField] private TimeCloneController timeCloneController;
    [SerializeField] private float recordingTimeLimit = 60;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject recordingIcon;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject events;
    [SerializeField] private MirrorMover[] mirrorMovers;
    #endregion

    #region Public fields
    public bool IsRecording
    {
        get{return recording;}
    }
    #endregion

    #region Private fields
    private Aim aim;
    private TimeCloneDevice activeCloneMachine;
    private List<RecordedCommand> commands;
    private bool recording;
    private Vector3 recordingStartPos;
    private Weapon startingWeapon;
    private float accumulatedTime;
    private float timer;
    #endregion

    void Start()
    {
        aim = transform.GetChild(0).GetComponent<Aim>();
        recording = false;
        commands = new List<RecordedCommand>();
        accumulatedTime = 0;
        timer = 0;
    }

    void Update()
    {
        if(timer > 0) 
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
        }
        else if(recording)
        {
            recording = false;
            GetComponent<PlayerController>().RecordingCancelled();
            StopRecording();
        } 
    }

    public void StartRecording(TimeCloneDevice nearbyCloneMachine, Weapon weapon)
    {
        recording = true;
        PhysicsObject.UpdateAllInitialPositions();
        recordingStartPos = transform.position;
        activeCloneMachine = nearbyCloneMachine;
        activeCloneMachine.SetActiveLight();
        startingWeapon = weapon;
        timer = recordingTimeLimit;

        timeCloneController.PlayBack(activeCloneMachine);
        if(enemyManager) enemyManager.CacheEnemyInfo();
        if(WeaponManager.weapons != null) WeaponManager.SetDefaultPosition();
        recordingIcon.SetActive(true);
        UpdateTimerText();
        foreach(MirrorMover mover in mirrorMovers)
        {
            mover.SetInitialPositions();
        }

        if(startingWeapon != null)
        {
            if(typeof(PhysicsRay).IsInstanceOfType(weapon))
            {
                PhysicsRay pr = (PhysicsRay) weapon;
                pr.SetStartingType();
            }
        }

        AudioManager.Instance.PlaySFX("StartRecording");
    }

    public void StopRecording()
    {
        recording = false;
        accumulatedTime = 0;
        transform.parent = null;
        timer = 0;
        AudioManager.Instance.PlaySFX("EndRecording");

        GameObject endingWeapon = (aim.CurrentWeapon ? aim.CurrentWeapon.gameObject : null);
        activeCloneMachine.StoreClone(new List<RecordedCommand>(commands), recordingStartPos);

        commands.Clear();
        activeCloneMachine = null;
        recordingIcon.SetActive(false);
        timeCloneController.RemoveAllActiveClones();

        if(aim.CurrentWeapon != null) aim.DropWeapon();
        if(WeaponManager.weapons != null) WeaponManager.ResetAllWeapons();
        if(startingWeapon != null) aim.PickUpWeapon(startingWeapon);


        StartCoroutine(TeleportAnimation());
        
    }

    IEnumerator TeleportAnimation()
    {
        // Hide the player
        DisablePlayer(false);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        aim.gameObject.SetActive(false);

        // Start the teleportation animation
        GameObject tp = Instantiate(teleportPrefab, transform.position, transform.rotation);
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
        if(enemyManager)
        {
            enemyManager.ResetEnemies();
            enemyManager.ResetCurrentBoss();
        }

        foreach(MirrorMover mover in mirrorMovers)
        {
            mover.ResetPositions();
        }

        // Do the second half of the teleport
        transform.position = recordingStartPos;
        tp.transform.position = recordingStartPos;
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
        DisablePlayer(true);
        GetComponent<Rigidbody2D>().gravityScale = 3;
    }

    void DisablePlayer(bool enabled)
    {
        GetComponent<SpriteRenderer>().enabled = enabled;
        GetComponent<PlayerMovement>().enabled = enabled;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = enabled;
    }

    public void ResetAllEvents()
    {
        ResetEvents(events);
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

    public void AddCommand(Vector2 movement, bool jumping, float angle, bool shooting, float raySwitchValue, bool grabbing, bool movingMirror, float mirrorMoveValue, GameObject newWeapon = null)
    {
        accumulatedTime += Time.fixedDeltaTime;
        RecordedCommand command = new RecordedCommand(movement, jumping, angle, shooting, accumulatedTime, raySwitchValue, grabbing, movingMirror, mirrorMoveValue, newWeapon);
        commands.Add(command);
    }

    public void CancelRecording(bool playerDied = false)
    {
        if(recording)
        {
            recording = false;
            accumulatedTime = 0;
            commands.Clear();
            activeCloneMachine.SetEmptyLight();
            activeCloneMachine = null;
            recordingIcon.SetActive(false);
            timeCloneController.RemoveAllActiveClones();
            GetComponent<PlayerController>().RecordingCancelled();
            if(playerDied)
            {
                ResetAllEvents();
                PhysicsObject.ResetAllPhysics(true, true);
                GetComponent<PlayerStatus>().DestroyAllProjectiles();
                if(enemyManager)
                {
                    enemyManager.ResetEnemies();
                    enemyManager.ResetCurrentBoss();
                }
                if(aim.CurrentWeapon != null) aim.DropWeapon();
                if(WeaponManager.weapons != null) WeaponManager.ResetAllWeapons();
                if(startingWeapon != null) aim.PickUpWeapon(startingWeapon);

                foreach(MirrorMover mover in mirrorMovers)
                {
                    mover.ResetPositions();
                }
            }
            else
            {
                foreach(MirrorMover mover in mirrorMovers)
                {
                    mover.ExitMover();
                }
            }
        }
    }

    void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(timer / 60.0f);
        float seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
