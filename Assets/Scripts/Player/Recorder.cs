using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Recorder : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private TimeCloneController timeCloneController;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject recordingIcon;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject events;
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
    private PhysicsRay.RayType startingRayType;
    private bool recording;
    private Vector3 recordingStartPos;
    private bool startedWithGun;
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
        startingRayType = PhysicsRay.RayType.None;
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
            StopRecording();
            GetComponent<PlayerController>().RecordingCancelled();
        } 
    }

    public void StartRecording(TimeCloneDevice nearbyCloneMachine, Weapon weapon)
    {
        recording = true;
        PhysicsObject.UpdateAllInitialPositions();
        recordingStartPos = transform.position;
        activeCloneMachine = nearbyCloneMachine;
        startedWithGun = weapon != null;
        timer = 60;

        timeCloneController.PlayBack(activeCloneMachine);
        if(enemyManager) enemyManager.CacheEnemyInfo();
        recordingIcon.SetActive(true);
        UpdateTimerText();

        if(startedWithGun)
        {
            if(typeof(PhysicsRay).IsInstanceOfType(weapon))
            {
                PhysicsRay pr = (PhysicsRay) weapon;
                startingRayType = pr.CurrentRay;
            }
        }
    }

    public void StopRecording()
    {
        recording = false;
        accumulatedTime = 0;
        transform.position = recordingStartPos;
        timer = 0;

        GameObject weapon = (aim.CurrentWeapon ? aim.CurrentWeapon.gameObject : null);
        activeCloneMachine.StoreClone(new List<RecordedCommand>(commands), weapon, recordingStartPos, startingRayType);
        commands.Clear();
        activeCloneMachine = null;
        recordingIcon.SetActive(false);
        timeCloneController.RemoveAllActiveClones();
        ResetAllObjects();
        PhysicsObject.ResetAllPhysics(true);
        GetComponent<PlayerStatus>().DestroyAllProjectiles();
        if(enemyManager)
        {
            enemyManager.ResetEnemies();
            enemyManager.ResetCurrentBoss();
        }
        if(!startedWithGun)
        {
            aim.ResetWeapon();
        }
    }

    public void ResetAllObjects()
    {
        ResetObjects(events);
    }

    void ResetObjects(GameObject parent)
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
                ResetObjects(child);
            }
        }
    }

    public void AddCommand(Vector2 movement, bool jumping, float angle, bool shooting, bool hasWeapon, float raySwitchValue)
    {
        accumulatedTime += Time.fixedDeltaTime;
        RecordedCommand command = new RecordedCommand(movement, jumping, angle, shooting, accumulatedTime, hasWeapon, raySwitchValue);
        commands.Add(command);
    }

    public void CancelRecording()
    {
        if(recording)
        {
            recording = false;
            accumulatedTime = 0;
            commands.Clear();
            activeCloneMachine = null;
            recordingIcon.SetActive(false);
            timeCloneController.RemoveAllActiveClones();
            GetComponent<PlayerController>().RecordingCancelled();
        }
    }

    void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(timer / 60.0f);
        float seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
