using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeCloneDevice : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject recordingTemplate;
    [SerializeField] private bool unstable = false;
    #endregion

    #region Private fields
    private const float NO_CLONE_LIGHT_INTENSITY = 1f;
    private const float STORED_CLONE_LIGHT_INTENSITY = 2f;
    private const float RECORDING_CLONE_LIGHT_INTENSITY = 3f;
    private Light2D light;
    private float baseIntensity;
    private List<RecordedCommand> commands;
    private Vector3 spawn;
    private TimeCloneController timeCloneController;
    #endregion
    
    void Awake()
    {
        commands = null;
        light = GetComponentInChildren<Light2D>();
        baseIntensity = light.intensity;
    }

    void Start()
    {
        timeCloneController = transform.parent.GetComponent<TimeCloneController>();
    }

    public void StoreClone(List<RecordedCommand> commands, Vector3 spawn)
    {
        this.commands = commands;
        this.spawn = spawn;
        light.intensity = baseIntensity * STORED_CLONE_LIGHT_INTENSITY;
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
            if(unstable) ec.MakeUnstable();
            Color cloneColour = GetComponent<SpriteRenderer>().color;
            cloneColour.a = 0.59f;
            ec.GetComponent<DamageFlash>().SetBaseColour(cloneColour);
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
            light.intensity = baseIntensity * NO_CLONE_LIGHT_INTENSITY;
        }
    }

    public void SetActiveLight()
    {
        light.intensity = baseIntensity * RECORDING_CLONE_LIGHT_INTENSITY;
    }

    public void SetEmptyLight()
    {
        light.intensity = baseIntensity * NO_CLONE_LIGHT_INTENSITY;
    }
}
