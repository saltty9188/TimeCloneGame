using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{

    #region Private fields
    private Rigidbody2D rigidbody2D;
    private PlayerMovement playerMovement;
    private PlayerControls controls;
    private Aim aim;
    private Recorder recorder;
    private TimeCloneDevice nearbyCloneMachine;
    private MirrorMover nearbyMirrorMover;
    private InputDevice controlScheme;
    private Vector2 aimVector;
    private Vector2 movement;
    private bool jumping;
    private bool shooting;
    private float raySwitchValue;
    private bool recording;
    private GameObject prevWeapon;
    private bool movingMirrors;
    private bool mirrorButtonHeld;
    #endregion

    void Awake()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        aim = gameObject.GetComponentInChildren<Aim>();
        recorder = GetComponent<Recorder>();
        controls = new PlayerControls();

        recording = false;
        prevWeapon = null;
        nearbyCloneMachine = null;
        nearbyMirrorMover = null;
        movingMirrors = false;

        //Jumping
        controls.Gameplay.Jump.performed += ctx => 
            {
                jumping = true;
            };
        controls.Gameplay.Jump.canceled += ctx => 
            {
                jumping = false;
            };

        //Movement
        controls.Gameplay.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movement = Vector2.zero;

        //Detect Input Device
        InputSystem.onActionChange += (obj, change) =>
            {
                if (change == InputActionChange.ActionPerformed)
                {
                    var inputAction = (InputAction) obj;
                    var lastControl = inputAction.activeControl;
                    controlScheme = lastControl.device;
                }
            };
        controlScheme = new InputDevice();

        //Aiming
        controls.Gameplay.AimStick.performed += ctx => aimVector = ctx.ReadValue<Vector2>();
        controls.Gameplay.AimStick.canceled += ctx => aimVector = Vector2.zero;

        controls.Gameplay.AimMouse.performed += ctx => aimVector = ctx.ReadValue<Vector2>();

        //Shooting
        controls.Gameplay.Shoot.performed += ctx => 
            {
                shooting = true;
            };

        controls.Gameplay.Shoot.canceled += ctx => 
            {
                shooting = false;
            };

        //recording
        controls.Gameplay.Record.performed += ctx =>
            {
                if(nearbyCloneMachine && !recording)
                {
                    recording = true;
                    recorder.StartRecording(nearbyCloneMachine, aim.CurrentWeapon);
                }
                else if(recording)
                {
                    recording = false;
                    prevWeapon = null;
                    recorder.StopRecording();
                }
            };
    
        //Physics type cycle
        controls.Gameplay.CyclePhysics.performed += ctx =>
            {
                raySwitchValue = ctx.ReadValue<Vector2>().y;

                if(raySwitchValue > 0)
                {
                    aim.NextRayType();
                }
                else if(raySwitchValue < 0)
                {
                    aim.PrevRayType();
                }
            }; 

        // Interact
        controls.Gameplay.Interact.performed += ctx =>
            {
                if(nearbyMirrorMover)
                {
                    if(!movingMirrors)
                    {
                        movingMirrors = true;
                        nearbyMirrorMover.StartMover();
                        rigidbody2D.isKinematic = true;
                        rigidbody2D.useFullKinematicContacts = true;
                        mirrorButtonHeld = true;
                    }
                }
            };

        controls.Gameplay.Interact.canceled += ctx => 
            {
                mirrorButtonHeld = false;
            };

        // Cycle between movable objects
        controls.Gameplay.CycleObjects.performed += ctx =>
            {
                if(movingMirrors && !mirrorButtonHeld)
                {
                    float moveValue = ctx.ReadValue<float>();
                    if(moveValue > 0)
                    {
                        nearbyMirrorMover.CycleNextObject();
                    }
                    else if(moveValue < 0)
                    {
                        nearbyMirrorMover.CyclePrevObject();
                    }
                }
            };

        // Cancel
        controls.Gameplay.Cancel.performed += ctx =>
            {
                if(movingMirrors)
                {
                    movingMirrors = false;
                    nearbyMirrorMover.ExitMover();
                    rigidbody2D.isKinematic = false;
                    rigidbody2D.useFullKinematicContacts = false;
                }
            };
    }

    void FixedUpdate()
    {

        float angle = 0;

        if(movingMirrors)
        {
            nearbyMirrorMover.Move(movement);
        }
        else
        {
            playerMovement.move(movement, jumping);
            angle = aim.Rotate(controlScheme.displayName, aimVector, shooting);
        }
        

        if(recording)
        {
            //Check if weapon changed while recording
            GameObject newWeapon = null;
            if(aim.CurrentWeapon != null && prevWeapon != aim.CurrentWeapon.gameObject) newWeapon = aim.CurrentWeapon.gameObject;
            prevWeapon = (aim.CurrentWeapon == null ? null : aim.CurrentWeapon.gameObject);

            recorder.AddCommand(movement, jumping, angle, shooting, raySwitchValue, newWeapon); // bool for mirror moving
        }

        //Reset raySwitchValue after it has been recorded
        raySwitchValue = 0;

        Debug.Log(nearbyCloneMachine == null);
    }

    void OnEnable() 
    {
        controls.Gameplay.Enable();      
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void RecordingCancelled()
    {
        recording = false;
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag != "Weapon")
        {
            nearbyCloneMachine = other.GetComponent<TimeCloneDevice>();
            nearbyMirrorMover = other.GetComponent<MirrorMover>();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag != "Weapon")
        {
            nearbyCloneMachine = null;
            nearbyMirrorMover = null;
        }
    }
}
