using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ExecuteCommands class is responsible for controlling a time-clone's actions.
/// </summary>
public class ExecuteCommands : MonoBehaviour
{
    #region Private struct
    // Used to keep track of weapons previously picked up by the time-clone
    private struct OldWeapon
    {
        public GameObject originalWeapon;
        public Weapon clonedWeapon;

        public OldWeapon(GameObject originalWeapon, Weapon clonedWeapon)
        {
            this.originalWeapon = originalWeapon;
            this.clonedWeapon = clonedWeapon;
        }
    }
    #endregion

    #region Private fields
    private Rigidbody2D _rigidbody2D;
    private PlayerMovement _playerMovement;
    private Aim _aim;
    private List<RecordedCommand> _recordedCommands;
    private int _commandIndex;
    private float _playbackTime;
    private bool _unstable; 
    private MirrorMover _nearbyMirrorMover;
    private bool _wasMovingMirror;
    private List<OldWeapon> _oldWeapons;
    private SpriteRenderer _spriteRenderer;
    // Flickering for unstable clones
    private float _flickerTime;
    #endregion

    void Awake()
    {
        _recordedCommands = null;
        _commandIndex = -1;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _aim = transform.GetChild(0).GetComponent<Aim>();
        _playbackTime = 0;
        _nearbyMirrorMover = null;
        _wasMovingMirror = false;
        _oldWeapons = new List<OldWeapon>();
    }

    void FixedUpdate()
    {
        // playback the commands while there are some in the List
        if(_commandIndex >= 0 && _commandIndex < _recordedCommands.Count)
        {
            RecordedCommand rc = _recordedCommands[_commandIndex];
            // Keep in time with the playback time
            if(rc.AccumulatedTime >= _playbackTime)
            {
                //Equip the weapon the clone just picked up
                if(rc.NewWeapon)
                {
                    // Switch back to the previous weapon if this one has already been picked up before
                    if(PickedUpBefore(rc.NewWeapon))
                    {
                        Weapon weapon = GetPreviousWeapon(rc.NewWeapon);
                        weapon.gameObject.SetActive(true);
                        GameObject oldWeapon = null;
                        if(_aim.CurrentWeapon != null) oldWeapon = _aim.CurrentWeapon.gameObject;
                        if(oldWeapon != null) oldWeapon.gameObject.SetActive(false);
                        _aim.PickUpWeapon(weapon);
                    }
                    else
                    {
                        GameObject newWeapon = Instantiate(rc.NewWeapon, new Vector3(), new Quaternion());
                        Weapon weaponScript = newWeapon.GetComponent<Weapon>();
                        Color baseCol = newWeapon.GetComponent<SpriteRenderer>().color;
                        baseCol.a = GetComponent<SpriteRenderer>().color.a;
                        newWeapon.GetComponent<SpriteRenderer>().color = baseCol;
                        
                        // swap weapons
                        GameObject oldWeapon = null;
                        if(_aim.CurrentWeapon != null) oldWeapon = _aim.CurrentWeapon.gameObject;
                        _aim.PickUpWeapon(weaponScript);
                        if(oldWeapon != null) oldWeapon.gameObject.SetActive(false);

                        // Keep the physics ray with the same starting ray type as the original
                        if(typeof(PhysicsRay).IsInstanceOfType(weaponScript))
                        {
                            PhysicsRay clonePhysicsRay = (PhysicsRay) weaponScript;
                            PhysicsRay originalPhysicsRay = rc.NewWeapon.GetComponent<PhysicsRay>();
                            clonePhysicsRay.SetRayType(originalPhysicsRay.CurrentRay); 
                        }

                        // Add the weapon to the list of old weapons
                        _oldWeapons.Add(new OldWeapon(rc.NewWeapon, weaponScript));
                    }
                } 

                // Use the mirror mover if the player was doing so while recording
                if(rc.MovingMirror)
                {
                    // Start the mover if its the first frame using it
                    if(!_wasMovingMirror && _nearbyMirrorMover)
                    {
                        _nearbyMirrorMover.StartMover(false);
                        _rigidbody2D.isKinematic = true;
                        _rigidbody2D.useFullKinematicContacts = true;
                    }
                    // Time-clone is not near a mirror mover so they must be out of synch
                    else if(!_wasMovingMirror && !_nearbyMirrorMover)
                    {
                        transform.parent.GetComponent<TimeCloneController>().OutOfSynch();
                        //skip to end
                        _commandIndex = _recordedCommands.Count;
                        return;
                    }

                    // Cycle to the next or previous movable object 
                    if(rc.MirrorMoveValue > 0)
                    {
                        _nearbyMirrorMover.CycleNextObject();
                    }
                    else if(rc.MirrorMoveValue < 0)
                    {
                        _nearbyMirrorMover.CyclePrevObject();
                    }
                    _nearbyMirrorMover.Move(rc.Movement);      
                }
                else
                {
                    // If the clone was using the mover last frame then exit from it now
                    if(_wasMovingMirror)
                    {
                        _nearbyMirrorMover.ExitMover();
                        _rigidbody2D.isKinematic = false;
                        _rigidbody2D.useFullKinematicContacts = false;
                    }

                    // cycle to the next ray type if needed
                    if(rc.RaySwitchValue > 0)
                    {
                        _aim.NextRayType();
                    }
                    else if (rc.RaySwitchValue < 0)
                    {
                        _aim.PrevRayType();
                    }
                    _playerMovement.Move(rc.Movement, rc.Jumping, rc.Grabbing);
                    _aim.RotateAndFire(rc.AimAngle, rc.Shooting);
                }

                _wasMovingMirror = rc.MovingMirror;
                _playbackTime += Time.fixedDeltaTime;
            }
            _commandIndex++;
        }
        else
        {
            //Stop moving clone once commands end
            _playerMovement.Move(Vector2.zero, false, false);
            RecordedCommand rc = _recordedCommands[_commandIndex - 1];
            DestroyPreviousWeapons(_aim.CurrentWeapon);
            _aim.RotateAndFire(rc.AimAngle, rc.Shooting);
        }

        // Wait a second before enabling collision with the player/other clones
        if(_playbackTime < 1.0f) Physics2D.IgnoreLayerCollision(8, 8, true);
        else Physics2D.IgnoreLayerCollision(8, 8, false);

    }

    void Update()
    {
        // Make the clone flicker if its unstable
        if(_unstable)
        {
            if(_flickerTime > 0)
            {
                _flickerTime -= Time.deltaTime;
                _spriteRenderer.enabled = false;
            }
            else
            {
                _spriteRenderer.enabled = true;
                int chance = Random.Range(0, 20);
                if(chance == 0)
                {
                    _flickerTime = 0.083f;
                }
            }
        }
    }

    /// <summary>
    /// Marks the time-clone as unstable, meaning it will kill the player or other time-clones on contact.
    /// </summary>
    public void MakeUnstable()
    {
        this._unstable = true;
        // Activate the red light
        transform.GetChild(2).gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the <see cref="RecordedCommand">RecordedCommands</see> to be played back.
    /// </summary>
    /// <param name="commands">The <see cref="RecordedCommand">RecordedCommands</see> to be played back.</param>
    public void SetCommands(List<RecordedCommand> commands)
    {
        _recordedCommands = commands;
        _commandIndex = 0;
    }

    /// <summary>
    /// Destroys the weapon the time-clone is holding.
    /// </summary>
    public void RemoveWeapon()
    {
        if(_aim.CurrentWeapon != null)
        {
            Destroy(_aim.CurrentWeapon.gameObject);
        }
    }

    // Checks if the weapon has been picked up before
    bool PickedUpBefore(GameObject newWeapon)
    {
        foreach(OldWeapon oldWeapon in _oldWeapons)
        {
            if(oldWeapon.originalWeapon == newWeapon) return true;
        }
        return false;
    }

    // If a weapon has been picked up before it is retrieved
    Weapon GetPreviousWeapon(GameObject originalWeapon)
    {
        foreach(OldWeapon oldWeapon in _oldWeapons)
        {
            if(oldWeapon.originalWeapon == originalWeapon) return oldWeapon.clonedWeapon;
        }

        return null;
    }

    // Destroys all of the weapons except for the currently held one
    void DestroyPreviousWeapons(Weapon ignoreWeapon)
    {
        foreach(OldWeapon oldWeapon in _oldWeapons)
        {
            if(oldWeapon.clonedWeapon != ignoreWeapon && oldWeapon.clonedWeapon != null) Destroy(oldWeapon.clonedWeapon.gameObject);
        }
    }

    // kill the clone and player if they collide when the clone is unstable
    void OnCollisionEnter2D(Collision2D other) 
    {
        if(_unstable && (other.GetContact(0).collider.tag == "Player" || other.GetContact(0).collider.tag == "Clone"))
        {
            other.GetContact(0).collider.GetComponent<PlayerStatus>().Die();
            GetComponent<PlayerStatus>().Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Weapon")
        {
            _nearbyMirrorMover = other.GetComponent<MirrorMover>();
        }
    }
    void OnDestroy()
    {
        DestroyPreviousWeapons(null);
    }
}
