using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The TimeCloneDevice class stores the <see cref="RecordedCommand">RecordedCommands</see> needed for a time-clone and has the ability to play them back.
/// </summary>
public class TimeCloneDevice : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The prefab for a time-clone.")]
    [SerializeField] private GameObject _recordingTemplate;
    [Tooltip("Does this time-clone device produce unstable time-clones?")]
    [SerializeField] private bool _unstable = false;
    #endregion

    #region Private fields
    // light intensity multipliers
    private const float NO_CLONE_LIGHT_INTENSITY = 1f;
    private const float STORED_CLONE_LIGHT_INTENSITY = 2f;
    private const float RECORDING_CLONE_LIGHT_INTENSITY = 3f;
    private Light2D _light;
    private float _baseIntensity;
    private List<RecordedCommand> _commands;
    private Vector3 _spawn;
    private TimeCloneController _timeCloneController;
    #endregion
    
    void Awake()
    {
        _commands = null;
        _light = GetComponentInChildren<Light2D>();
        _baseIntensity = _light.intensity;
    }

    void Start()
    {
        _timeCloneController = transform.parent.GetComponent<TimeCloneController>();
    }

    /// <summary>
    /// Stores the <see cref="RecordedCommand">RecordedCommands</see> and spawn position for a time-clone.
    /// </summary>
    /// <param name="commands">The <see cref="RecordedCommand">RecordedCommands</see> that will control the time-clone</param>
    /// <param name="spawn">The spawn position for the time-clone.</param>
    public void StoreClone(List<RecordedCommand> commands, Vector3 spawn)
    {
        this._commands = commands;
        this._spawn = spawn;
        _light.intensity = _baseIntensity * STORED_CLONE_LIGHT_INTENSITY;
    }

    /// <summary>
    /// Creates a time-clone and gives it the <see cref="RecordedCommand">RecordedCommands</see> to act out.
    /// </summary>
    public void Play()
    {
        if(_commands != null)
        {
            GameObject clone = Instantiate(_recordingTemplate, _spawn, new Quaternion());
            clone.transform.SetParent(transform.parent);
            
            _timeCloneController.activeClones.Add(clone);

            ExecuteCommands ec = clone.GetComponent<ExecuteCommands>();
            ec.SetCommands(new List<RecordedCommand>(_commands));
            if(_unstable) ec.MakeUnstable();
            // Colour code the clone
            Color cloneColour = GetComponent<SpriteRenderer>().color;
            cloneColour.a = 0.59f;
            ec.GetComponent<DamageFlash>().SetBaseColour(cloneColour);
        }
    }

    /// <summary>
    /// Clears the <see cref="RecordedCommand">RecordedCommands</see> stored in this TimeCloneDevice.
    /// </summary>
    public void Empty()
    {
        if(_commands != null)
        {
            _commands.Clear();
            _commands = null;
            _spawn = Vector3.zero;
            _light.intensity = _baseIntensity * NO_CLONE_LIGHT_INTENSITY;
        }
    }

    /// <summary>
    /// Sets the light attached to this TimeCloneDevice to be 3 times its base intensity.
    /// </summary>
    public void SetActiveLight()
    {
        _light.intensity = _baseIntensity * RECORDING_CLONE_LIGHT_INTENSITY;
    }

    /// <summary>
    /// Sets the light attached to this TimeCloneDevice to be its base intensity.
    /// </summary>
    public void SetEmptyLight()
    {
        _light.intensity = _baseIntensity * NO_CLONE_LIGHT_INTENSITY;
    }
}
