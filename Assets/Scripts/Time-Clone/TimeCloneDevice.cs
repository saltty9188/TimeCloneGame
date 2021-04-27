using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCloneDevice : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject recordingTemplate;
    [SerializeField] private bool unstable = false;
    #endregion

    #region Private fields
    private List<RecordedCommand> commands;
    private Vector3 spawn;
    private TimeCloneController timeCloneController;
    #endregion
    
    void Awake()
    {
        commands = null;
    }

    void Start()
    {
        timeCloneController = transform.parent.GetComponent<TimeCloneController>();
    }

    public void StoreClone(List<RecordedCommand> commands, Vector3 spawn)
    {
        this.commands = commands;
        this.spawn = spawn;
    }

    public void Play()
    {
        if(commands != null)
        {
            GameObject clone = Instantiate(recordingTemplate, spawn, new Quaternion());
            clone.transform.SetParent(transform.parent);
            
            timeCloneController.activeClones.Add(clone);

            ExecuteCommands ec = clone.GetComponent<ExecuteCommands>();
            ec.SetCommands(new List<RecordedCommand>(commands));
            ec.SetUnstable(unstable);
            Aim cloneArm = clone.transform.GetChild(0).GetComponent<Aim>();
        }
    }

    public void Empty()
    {
        if(commands != null)
        {
            commands.Clear();
            commands = null;
            spawn = Vector3.zero;
        }
    }
}
