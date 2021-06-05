using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCommands : MonoBehaviour
{

    #region Private struct
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
    private Rigidbody2D rigidbody2D;
    private PlayerMovement playerMovement;
    private Aim aim;
    private List<RecordedCommand> recordedCommands;
    private int commandIndex;
    private float playbackTime;
    private bool unstable; 
    private MirrorMover nearbyMirrorMover;
    private bool wasMovingMirror;
    private List<OldWeapon> oldWeapons;
    private SpriteRenderer spriteRenderer;
    private float flickerTime;
    #endregion

    void Awake()
    {
        recordedCommands = null;
        commandIndex = -1;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        aim = transform.GetChild(0).GetComponent<Aim>();
        playbackTime = 0;
        nearbyMirrorMover = null;
        wasMovingMirror = false;
        oldWeapons = new List<OldWeapon>();
    }

    void FixedUpdate()
    {
        if(commandIndex >= 0 && commandIndex < recordedCommands.Count)
        {
            RecordedCommand rc = recordedCommands[commandIndex];
            if(rc.time >= playbackTime)
            {
                //Equip the weapon the clone just picked up
                if(rc.newWeapon)
                {
                    if(PickedUpBefore(rc.newWeapon))
                    {
                        Weapon weapon = GetPreviousWeapon(rc.newWeapon);
                        weapon.gameObject.SetActive(true);
                        GameObject oldWeapon = null;
                        if(aim.CurrentWeapon != null) oldWeapon = aim.CurrentWeapon.gameObject;
                        if(oldWeapon != null) oldWeapon.gameObject.SetActive(false);
                        aim.PickUpWeapon(weapon);
                    }
                    else
                    {
                        GameObject newWeapon = Instantiate(rc.newWeapon, new Vector3(), new Quaternion());
                        Weapon weaponScript = newWeapon.GetComponent<Weapon>();
                        Color baseCol = newWeapon.GetComponent<SpriteRenderer>().color;
                        baseCol.a = GetComponent<SpriteRenderer>().color.a;
                        newWeapon.GetComponent<SpriteRenderer>().color = baseCol;
                        
                        GameObject oldWeapon = null;
                        if(aim.CurrentWeapon != null) oldWeapon = aim.CurrentWeapon.gameObject;
                        aim.PickUpWeapon(weaponScript);
                        if(oldWeapon != null) oldWeapon.gameObject.SetActive(false);

                        if(typeof(PhysicsRay).IsInstanceOfType(weaponScript))
                        {
                            PhysicsRay clonePhysicsRay = (PhysicsRay) weaponScript;
                            PhysicsRay originalPhysicsRay = rc.newWeapon.GetComponent<PhysicsRay>();
                            clonePhysicsRay.SetRayType(originalPhysicsRay.CurrentRay); 
                        }

                        oldWeapons.Add(new OldWeapon(rc.newWeapon, weaponScript));
                    }
                } 

                if(rc.movingMirror)
                {
                    if(!wasMovingMirror && nearbyMirrorMover)
                    {
                        nearbyMirrorMover.StartMover(false);
                        rigidbody2D.isKinematic = true;
                        rigidbody2D.useFullKinematicContacts = true;
                    }
                    else if(!wasMovingMirror && !nearbyMirrorMover)
                    {
                        transform.parent.GetComponent<TimeCloneController>().OutOfSynch();
                        //skip to end
                        commandIndex = recordedCommands.Count;
                        return;
                    }

                    if(rc.mirrorMoveValue > 0)
                    {
                        nearbyMirrorMover.CycleNextObject();
                    }
                    else if(rc.mirrorMoveValue < 0)
                    {
                        nearbyMirrorMover.CyclePrevObject();
                    }
                    nearbyMirrorMover.Move(rc.movement);      
                }
                else
                {
                    if(wasMovingMirror)
                    {
                        nearbyMirrorMover.ExitMover();
                        rigidbody2D.isKinematic = false;
                        rigidbody2D.useFullKinematicContacts = false;
                    }


                    if(rc.raySwitchValue > 0)
                    {
                        aim.NextRayType();
                    }
                    else if (rc.raySwitchValue < 0)
                    {
                        aim.PrevRayType();
                    }
                    playerMovement.move(rc.movement, rc.jumping, rc.grabbing);
                    aim.CloneRotate(rc.aimAngle, rc.shooting);
                }

                wasMovingMirror = rc.movingMirror;
                playbackTime += Time.fixedDeltaTime;
            }
            commandIndex++;
        }
        else
        {
            //Stop moving clone once commands end
            playerMovement.move(Vector2.zero, false, false);
            RecordedCommand rc = recordedCommands[commandIndex - 1];
            DestroyPreviousWeapons(aim.CurrentWeapon);
            aim.CloneRotate(rc.aimAngle, rc.shooting);
        }

        // Wait a second before enabling collision with the player/other clones
        if(playbackTime < 1.0f) Physics2D.IgnoreLayerCollision(8, 8, true);
        else Physics2D.IgnoreLayerCollision(8, 8, false);

    }

    void Update()
    {
        if(unstable)
        {
            if(flickerTime > 0)
            {
                flickerTime -= Time.deltaTime;
                spriteRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = true;
                int chance = Random.Range(0, 20);
                if(chance == 0)
                {
                    flickerTime = 0.083f;
                }
            }
        }
    }

    public void MakeUnstable()
    {
        this.unstable = true;
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void SetCommands(List<RecordedCommand> commands)
    {
        recordedCommands = commands;
        commandIndex = 0;
    }

    public void RemoveWeapon()
    {
        if(aim.CurrentWeapon != null)
        {
            Destroy(aim.CurrentWeapon.gameObject);
        }
    }

    bool PickedUpBefore(GameObject newWeapon)
    {
        foreach(OldWeapon oldWeapon in oldWeapons)
        {
            if(oldWeapon.originalWeapon == newWeapon) return true;
        }

        return false;
    }

    Weapon GetPreviousWeapon(GameObject originalWeapon)
    {
        foreach(OldWeapon oldWeapon in oldWeapons)
        {
            if(oldWeapon.originalWeapon == originalWeapon) return oldWeapon.clonedWeapon;
        }

        return null;
    }

    void DestroyPreviousWeapons(Weapon ignoreWeapon)
    {
        foreach(OldWeapon oldWeapon in oldWeapons)
        {
            if(oldWeapon.clonedWeapon != ignoreWeapon && oldWeapon.clonedWeapon != null) Destroy(oldWeapon.clonedWeapon.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(unstable && (other.GetContact(0).collider.tag == "Player" || other.GetContact(0).collider.tag == "Clone"))
        {
            other.GetContact(0).collider.GetComponent<PlayerStatus>().Die();
            GetComponent<PlayerStatus>().Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Weapon")
        {
            nearbyMirrorMover = other.GetComponent<MirrorMover>();
        }
    }
    void OnDestroy()
    {
        DestroyPreviousWeapons(null);
    }
}
