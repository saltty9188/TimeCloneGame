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
    private GameObject weapon;
    private Vector3 spawn;
    private TimeCloneController timeCloneController;
    private PhysicsRay.RayType startingRayType;
    #endregion
    
    void Awake()
    {
        commands = null;
        weapon = null;
    }

    void Start()
    {
        timeCloneController = transform.parent.GetComponent<TimeCloneController>();
    }

    public void StoreClone(List<RecordedCommand> commands, GameObject weapon, Vector3 spawn, PhysicsRay.RayType startingRayType)
    {
        this.commands = commands;
        this.weapon = weapon;
        this.spawn = spawn;
        this.startingRayType = startingRayType;
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
            if(weapon != null)
            {
                GameObject w = Instantiate(weapon, Vector3.zero, new Quaternion());
                Weapon s = w.GetComponent<Weapon>();
                cloneArm.PickUpWeapon(s);
                Color baseCol = w.GetComponent<SpriteRenderer>().color;
                baseCol.a = 0.6f;
                w.GetComponent<SpriteRenderer>().color = baseCol;

                if(typeof(PhysicsRay).IsInstanceOfType(s))
                {
                    PhysicsRay clonePhysicsRay = (PhysicsRay) s;
                    clonePhysicsRay.SetRayType(startingRayType); 
                }
            }
        }
    }

    public void Empty()
    {
        if(commands != null)
        {
            commands.Clear();
            commands = null;
            weapon = null;
            spawn = Vector3.zero;
        }
    }
}
