using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The PlayerController class is responsible for handling input from the player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The pause menu used in this level.")]
    [SerializeField] private PauseMenu _pauseMenu;
    #endregion

    #region Public fields
    /// <value>A string representing the current control scheme in use by the player.</value>
    public static string ControlScheme
    {
        get {return _controlScheme;}
    }

    /// <value>Returns the PlayerControls in use by the player.</value>
    public PlayerControls CurrentControls
    {
        get {return _controls;}
    }
    
    /// <value>Returns whether or not the player is currently using a MirrorMover.</value>
    public bool MovingMirrors
    {
        get {return _movingMirrors;}
    }
    #endregion

    #region Private fields
    private static string _controlScheme;
    private Rigidbody2D _rigidbody2D;
    private ToolTips _toolTips;
    private PlayerMovement _playerMovement;
    private PlayerControls _controls;
    private Aim _aim;
    private Recorder _recorder;
    private TimeCloneDevice _nearbyCloneMachine;
    private MirrorMover _nearbyMirrorMover;
    private Vector2 _aimVector;
    private Vector2 _movement;
    private bool _jumping;
    private bool _shooting;
    private bool _grabbing;
    private float _raySwitchValue;
    private bool _recording;
    private GameObject _prevWeapon;
    private bool _movingMirrors;
    float _mirrorMoveValue;
    private bool _mirrorButtonHeld;
    #endregion

    void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _toolTips = GetComponent<ToolTips>();
        _playerMovement = gameObject.GetComponent<PlayerMovement>();
        _aim = gameObject.GetComponentInChildren<Aim>();
        _recorder = GetComponent<Recorder>();
        _controls = new PlayerControls();
        _controlScheme = "KeyboardMouse";

        _recording = false;
        _prevWeapon = null;
        _nearbyCloneMachine = null;
        _nearbyMirrorMover = null;
        _movingMirrors = false;


        //Jumping
        _controls.Gameplay.Jump.performed += ctx => 
            {
                _jumping = true;
            };
        _controls.Gameplay.Jump.canceled += ctx => 
            {
                _jumping = false;
            };



        //Movement
        _controls.Gameplay.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Move.canceled += ctx => _movement = Vector2.zero;

        //Detect Input Device
        InputSystem.onActionChange += (obj, change) =>
            {
                if (change == InputActionChange.ActionPerformed)
                {
                    var inputAction = (InputAction) obj;
                    var lastControl = inputAction.activeControl;
                    var lastDevice = lastControl.device;
                    SetControlScheme(lastDevice);
                }
            };

        //Aiming
        _controls.Gameplay.AimStick.performed += ctx => _aimVector = ctx.ReadValue<Vector2>();
        _controls.Gameplay.AimStick.canceled += ctx => _aimVector = Vector2.zero;

        _controls.Gameplay.AimMouse.performed += ctx => _aimVector = ctx.ReadValue<Vector2>();

        //Shooting
        _controls.Gameplay.Shoot.performed += ctx => 
            {
                _shooting = true;
            };

        _controls.Gameplay.Shoot.canceled += ctx => 
            {
                _shooting = false;
            };

        //recording
        _controls.Gameplay.Record.performed += ctx =>
            {
                if(_nearbyCloneMachine && !_recording && !_movingMirrors)
                {
                    _recording = true;
                    _prevWeapon = null;
                    _recorder.StartRecording(_nearbyCloneMachine, _aim.CurrentWeapon);
                }
                else if(_recording && !_movingMirrors)
                {
                    _recording = false;
                    _prevWeapon = null;
                    _recorder.StopRecording();
                }
            };
    
        //Physics type cycle
        _controls.Gameplay.CyclePhysics.performed += ctx =>
            {
                _raySwitchValue = ctx.ReadValue<Vector2>().y;

                if(_raySwitchValue > 0)
                {
                    _aim.NextRayType();
                }
                else if(_raySwitchValue < 0)
                {
                    _aim.PrevRayType();
                }
            }; 

        // Interact
        _controls.Gameplay.Interact.performed += ctx =>
            {
                if(_nearbyMirrorMover)
                {
                    if(!_movingMirrors)
                    {
                        _movingMirrors = true;
                        _nearbyMirrorMover.StartMover();
                        _rigidbody2D.isKinematic = true;
                        _rigidbody2D.useFullKinematicContacts = true;
                        _rigidbody2D.velocity = Vector2.zero;
                        _mirrorButtonHeld = true;
                        GetComponent<Animator>().SetFloat("Speed", 0);
                    }
                }
            };

        _controls.Gameplay.Interact.canceled += ctx => 
            {
                _mirrorButtonHeld = false;
            };

        // Cycle between movable objects
        _controls.Gameplay.CycleObjects.performed += ctx =>
            {
                if(_movingMirrors && !_mirrorButtonHeld)
                {
                    _mirrorMoveValue = ctx.ReadValue<float>();
                    if(_mirrorMoveValue > 0)
                    {
                        _nearbyMirrorMover.CycleNextObject();
                    }
                    else if(_mirrorMoveValue < 0)
                    {
                        _nearbyMirrorMover.CyclePrevObject();
                    }
                }
            };

        // Cancel
        _controls.Gameplay.Cancel.performed += ctx =>
            {
                if(_movingMirrors)
                {
                    _movingMirrors = false;
                    _nearbyMirrorMover.ExitMover();
                    _rigidbody2D.isKinematic = false;
                    _rigidbody2D.useFullKinematicContacts = false;
                }
            };

        // Grab
        _controls.Gameplay.Grab.performed += ctx =>
            {
                _grabbing = true;
            };

        _controls.Gameplay.Grab.canceled += ctx =>
            {
                _grabbing = false;
            };


        //pause
        _controls.Gameplay.Pause.performed += ctx =>
            {
                if(Time.timeScale > 0)
                {
                    _pauseMenu.Pause();
                }
                else
                {
                    _pauseMenu.Resume();
                }
            };
    }

    void FixedUpdate()
    {
        float angle = 0;

        // Use the mirror mover
        if(_movingMirrors)
        {
            _nearbyMirrorMover.Move(_movement);
            _toolTips.SetMoverToolTips(_nearbyMirrorMover);
        }
        else
        {
            // move the player
            _playerMovement.Move(_movement, _jumping, _grabbing);
            angle = _aim.CalculateRotation(_aimVector);
            _aim.RotateAndFire(angle, _shooting);
        }

        // Record the input
        if(_recording)
        {
            //Check if weapon changed while recording
            GameObject newWeapon = null;
            if(_aim.CurrentWeapon != null && _prevWeapon != _aim.CurrentWeapon.gameObject) newWeapon = _aim.CurrentWeapon.gameObject;
            _prevWeapon = (_aim.CurrentWeapon == null ? null : _aim.CurrentWeapon.gameObject);

            _recorder.AddCommand(_movement, _jumping, angle, _shooting, _raySwitchValue, _grabbing, _movingMirrors, _mirrorMoveValue, newWeapon); // bool for mirror moving
        }

        //Reset move values after they have been recorded
        _raySwitchValue = 0;
        _mirrorMoveValue = 0;
    }

    void OnEnable() 
    {
        _controls.Gameplay.Enable();      
    }

    void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    /// <summary>
    /// Stops the recording if the timer ran out or a CheckPoint was crossed.
    /// </summary>
    public void RecordingCancelled()
    {
        _recording = false;
        StopMovingMirrors();
    }

    /// <summary>
    /// Safely stops the player using the MirrorMover if their recording timer ran out or they returned to the last CheckPoint.
    /// </summary>
    public void StopMovingMirrors()
    {
        if(_movingMirrors)
        {
            _movingMirrors = false;
            _nearbyMirrorMover.ExitMover();
            _nearbyMirrorMover.ResetPositions();
            _rigidbody2D.isKinematic = false;
            _rigidbody2D.useFullKinematicContacts = false;
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "MirrorMover")
        {
            _nearbyMirrorMover = other.GetComponent<MirrorMover>();
        }
        else if(other.tag == "CloneDevice")
        {
            _nearbyCloneMachine = other.GetComponent<TimeCloneDevice>();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "MirrorMover")
        {
            _nearbyMirrorMover = null;
        }
        else if(other.tag == "CloneDevice")
        {
            _nearbyCloneMachine = null;
        }
    }
    
    void SetControlScheme(InputDevice device)
    {
        if(device.displayName == "Mouse" || device.displayName == "Keyboard")
        {
            _controlScheme = "KeyboardMouse";
        }
        else
        {
            _controlScheme = "Gamepad";
        }
    }
}
