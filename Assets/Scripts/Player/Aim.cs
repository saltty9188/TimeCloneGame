using UnityEngine;
using TMPro;

/// <summary>
/// The Aim class is responsible for managing <see cref="Weapon">Weapons</see> equipped by the player and aiming the direction they are fired.
/// </summary>
public class Aim : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The arm sprites the player switches between depending on the weapon equipped.")]
    [SerializeField] private Sprite[] _armSprites;
    [Tooltip("The text displaying the current Physics Ray Type selected.")]
    [SerializeField] private TextMeshProUGUI _rayTypeText;
    [Tooltip("What surfaces can stop the player from firing inside them?")]
    [SerializeField] private LayerMask _aimBlockers;
    #endregion

    #region Public fields
    /// <value>The current Weapon held by the player or null if there is none.</value>
    public Weapon CurrentWeapon
    {
        get {return _weapon;}
    }
    #endregion

    #region Private fields
    private Weapon _weapon;
    private bool _facingRight;
    private Sprite _weaponArm;
    #endregion
    
    void Awake()
    {
        _facingRight = true;
        SetRayTypeText();
        _weaponArm = _armSprites[0];
    }

    /// <summary>
    /// Calculates the arm's rotation from the input direction vector.
    /// </summary>
    /// <param name="inputDirection">The co-ordinates od the mouse if the player is using keyboard and mouse or the position of the right stick on controller.</param>
    /// <returns>The calculated angle for the arm to be rotated.</returns>
    public float CalculateRotation(Vector2 inputDirection)
    {
        float armRotation = 0;
        // only rotate the arm if the script is enabled and the character isn't pushing
        if(this.enabled && GetComponent<SpriteRenderer>().sprite != _armSprites[2])
        {
            if(PlayerController.ControlScheme == "KeyboardMouse")
            {
                // Convert the mouse screen position to a world position
                Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(inputDirection);

                //Make the aim more accurate to the weapon's fire point if one is equipped
                float aimVerticalOffset = 0;
                if(CurrentWeapon != null)
                {
                    aimVerticalOffset = _weapon.transform.localPosition.y + _weapon.transform.GetChild(0).localPosition.y;
                }
                
                armRotation = Mathf.Atan2(mousePos2D.y - transform.position.y - aimVerticalOffset, mousePos2D.x - transform.position.x) * Mathf.Rad2Deg;
            }
            else
            {
                // Leave arm in its last position if player isn't touching the stick
                if(inputDirection != Vector2.zero) armRotation = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            }
        }
        return armRotation;
    }

    /// <summary>
    /// Rotates the arm by the given angle and fires the equipped weapon if specified.
    /// </summary>
    /// <seealso cref="CalculateRotation(Vector2)"/>
    /// <seealso cref="Weapon.Shoot(Quaternion)"/>
    /// <param name="armRotation">The input angle for the arm to be rotated about.</param>
    /// <param name="shoot">Whether or not the player is shooting.</param>
    public void RotateAndFire(float armRotation, bool shoot)
    {
        // only rotate the arm if the script is enabled and the character isn't pushing
        if(this.enabled && GetComponent<SpriteRenderer>().sprite != _armSprites[2])
        {
            _facingRight = transform.parent.localScale.x > 0;
            // Offset the angle by 180 degrees if the player sprite is flipped
            float angleOffset = (_facingRight ? 0 : 180);
            transform.rotation = Quaternion.Euler(0,0, armRotation + angleOffset);

            // Flip the sprite if the aim was behind the player
            if(Mathf.Abs(armRotation) > 90 && _facingRight)
            {
                Flip();
            } 
            if(Mathf.Abs(armRotation) < 90 && !_facingRight)
            {
                Flip();
            }

            if(shoot)
            {
                Shoot();
            }
        }
    }

    // Flips the player sprite
    void Flip()
    {
        Vector3 temp = gameObject.transform.parent.localScale;
        temp.x *= -1;
        gameObject.transform.parent.localScale = temp;
    }

    /// <summary>
    /// Picks up the given Weapon and drops the currently equipped Weapon if needed.
    /// </summary>
    /// <param name="weapon">The Weapon to be picked up.</param>
    public void PickUpWeapon(Weapon weapon)
    {
        if(this._weapon != null)
        {
            DropWeapon(weapon.transform.position);
        }

        this._weapon = weapon;
        weapon.PickUp(gameObject);
        // Normal arm sprite for pistol and blaster, second arm sprite for the physics ray
        if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
        {
            _weaponArm = _armSprites[1];
        }
        else
        {
            _weaponArm = _armSprites[0];
        }

        GetComponent<SpriteRenderer>().sprite = _weaponArm;
        SetRayTypeText();
    }

    /// <summary>
    /// Drops the currently equipped weapon.
    /// </summary>
    /// <param name="dropPosition">The position for the weapon to be placed after it's been dropped.</param>
    public void DropWeapon(Vector3 dropPosition)
    {
        _weapon.gameObject.SetActive(true);
        _weapon.Drop(dropPosition);
        this._weapon = null;
        SetRayTypeText();
    }

    void Shoot()
    {
        if(_weapon != null && _weapon.gameObject.activeSelf)
        {
            Vector3 firePoint = _weapon.transform.GetChild(0).position;

            GameObject firedProjectile = null;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, firePoint - transform.position, 
                                                 Vector3.Distance(transform.position, firePoint), _aimBlockers);
            if(!(hit && hit.collider.gameObject != transform.parent.gameObject))
            {
                firedProjectile = _weapon.Shoot(transform.rotation);
            }

            // Stores the projectile in the projectile parent so it can be destroyed when needed
            if(firedProjectile) transform.parent.GetComponent<PlayerStatus>().AddProjectile(firedProjectile);
        }
    }

    /// <summary>
    /// Changes the arm sprite to the grabbing arm if the player is nearby a box
    /// </summary>
    /// <param name="nearBox">Whether or not the player is nearby a box.</param>
    public void GrabArm(bool nearBox)
    {
        if(nearBox)
        {
            GetComponent<SpriteRenderer>().sprite = _armSprites[2];
            transform.rotation = new Quaternion();
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = _weaponArm;
        }

        // hide the weapon if grabbing
        if(_weapon != null)
        {
            _weapon.gameObject.SetActive(!nearBox);
        }
    }  

    /// <summary>
    /// Cycles to the next available <see cref="PhysicsRay.RayType"/>.
    /// </summary>
    public void NextRayType()
    {
        if(_weapon != null && typeof(PhysicsRay).IsInstanceOfType(_weapon))
        {
            PhysicsRay pr = (PhysicsRay) _weapon;
            pr.NextRayType();
        }
        SetRayTypeText();
    }

    /// <summary>
    /// Cycles to the previous available <see cref="PhysicsRay.RayType"/>.
    /// </summary>
    public void PrevRayType()
    {
        if(_weapon != null && typeof(PhysicsRay).IsInstanceOfType(_weapon))
        {
            PhysicsRay pr = (PhysicsRay) _weapon;
            pr.PrevRayType();
        }
        SetRayTypeText();
    }

    // Adjust the text on the screen displaying the current ray type
    void SetRayTypeText()
    {
        if(_rayTypeText != null)
        {
            if(_weapon != null && typeof(PhysicsRay).IsInstanceOfType(_weapon))
            {
                _rayTypeText.gameObject.SetActive(true);
                PhysicsRay pr = (PhysicsRay) _weapon;
                pr.SetUIText(_rayTypeText);
            }
            else
            {
                _rayTypeText.gameObject.SetActive(false);
            }
        }
    }
}
