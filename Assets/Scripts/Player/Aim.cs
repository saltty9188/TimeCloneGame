using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Aim : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private Sprite[] armSprites;
    [SerializeField] private Weapon weapon;
    [SerializeField] private TextMeshProUGUI rayTypeText;
    [SerializeField] private LayerMask ignorePlayerAndClone;
    #endregion

    #region Public fields
    public Weapon CurrentWeapon
    {
        get {return weapon;}
    }
    #endregion

    #region Private fields
    private bool facingRight;
    private float angleOffset;
    private float armRotation;
    private Projectile shoot;
    private Sprite weaponArm;
    #endregion
    
    void Awake()
    {
        facingRight = true;
        angleOffset = 0;
        armRotation = 0;
        SetRayTypeText();
        weaponArm = armSprites[0];
    }

    public float Rotate(Vector2 inputDirection, bool shoot)
    {
        // only rotate the arm if the script is enabled and the character isn't pushing
        if(this.enabled && GetComponent<SpriteRenderer>().sprite != armSprites[2])
        {
            if(PlayerController.controlScheme == "KeyboardMouse")
            {
                Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(inputDirection);

                //Make the aim more accurate to the weapon's fire point if one is equipped
                float aimVerticalOffset = 0;
                if(CurrentWeapon != null)
                {
                    aimVerticalOffset = weapon.transform.localPosition.y + weapon.transform.GetChild(0).localPosition.y;
                }
                
                armRotation = Mathf.Atan2(mousePos2D.y - transform.position.y - aimVerticalOffset, mousePos2D.x - transform.position.x) * Mathf.Rad2Deg;
            }
            else
            {
                // Leave arm in its last position if player isn't touching the stick
                if(inputDirection != Vector2.zero) armRotation = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            }

            facingRight = transform.parent.localScale.x > 0;
            angleOffset = (facingRight ? 0 : 180);

            if(Mathf.Abs(armRotation) > 90 && facingRight)
            {
                Flip();
                //angleOffset = 180;
            }
                
            if(Mathf.Abs(armRotation) < 90 && !facingRight)
            {
                Flip();
                //angleOffset = 0;
            }

            transform.rotation = Quaternion.Euler(0,0, armRotation + angleOffset);

            if(shoot)
            {
                Shoot();
            }
        }
        
        return armRotation;
    }

    public void CloneRotate(float armRotation, bool shoot)
    {
        // only rotate the arm if the script is enabled and the character isn't pushing
        if(this.enabled && GetComponent<SpriteRenderer>().sprite != armSprites[2])
        {
            facingRight = transform.parent.localScale.x > 0;
            angleOffset = (facingRight ? 0 : 180);

            if(Mathf.Abs(armRotation) > 90 && facingRight)
            {
                Flip();
                //angleOffset = 180;
            }
                
            if(Mathf.Abs(armRotation) < 90 && !facingRight)
            {
                Flip();
                //angleOffset = 0;
            }
            
            transform.rotation = Quaternion.Euler(0,0, armRotation + angleOffset);

            if(shoot)
            {
                Shoot();
            }
        }
    }

    void Flip()
    {
        Vector3 temp = gameObject.transform.parent.localScale;
        temp.x *= -1;
        gameObject.transform.parent.localScale = temp;
    }

    public void PickUpWeapon(Weapon weapon)
    {
        if(this.weapon != null)
        {
            DropWeapon();
        }
        this.weapon = weapon;
        weapon.PickUp(gameObject);
        if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
        {
            weaponArm = armSprites[1];
        }
        else
        {
            weaponArm = armSprites[0];
        }

        GetComponent<SpriteRenderer>().sprite = weaponArm;
        SetRayTypeText();
    }

    public void DropWeapon()
    {
        weapon.Drop(gameObject);
        this.weapon = null;
        SetRayTypeText();
    }

    public void Shoot()
    {
        if(weapon != null && weapon.gameObject.activeSelf)
        {
            Vector3 firePoint = weapon.transform.GetChild(0).position;

            GameObject firedProjectile = null;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, firePoint - transform.position, 
                                                 Vector3.Distance(transform.position, firePoint), ignorePlayerAndClone);
            if(!(hit && hit.collider.gameObject != transform.parent.gameObject))
            {
                firedProjectile = weapon.Shoot(transform.rotation);
            }

            if(firedProjectile) transform.parent.GetComponent<PlayerStatus>().AddProjectile(firedProjectile);
        }
    }

    public void GrabArm(bool nearBox)
    {
        if(nearBox)
        {
            GetComponent<SpriteRenderer>().sprite = armSprites[2];
            transform.rotation = new Quaternion();
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = weaponArm;
        }

        if(weapon != null)
        {
            weapon.gameObject.SetActive(!nearBox);
        }
    }

    public void ResetWeapon()
    {
        if(weapon != null)
        {
            weapon.ResetWeapon();
            weapon = null;
        }
        SetRayTypeText();
    }   

    public void NextRayType()
    {
        if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
        {
            PhysicsRay pr = (PhysicsRay) weapon;
            pr.NextRayType();
        }
        SetRayTypeText();
    }

    public void PrevRayType()
    {
        if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
        {
            PhysicsRay pr = (PhysicsRay) weapon;
            pr.PrevRayType();
        }
        SetRayTypeText();
    }

    void SetRayTypeText()
    {
        if(rayTypeText != null)
        {
            if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
            {
                rayTypeText.gameObject.SetActive(true);
                PhysicsRay pr = (PhysicsRay) weapon;
                pr.SetUIText(rayTypeText);
            }
            else
            {
                rayTypeText.gameObject.SetActive(false);
            }
        }
    }

    public void ResetRayType()
    {
        if(weapon != null && typeof(PhysicsRay).IsInstanceOfType(weapon))
        {
            PhysicsRay pr = (PhysicsRay) weapon;
            pr.ResetRayType();
        }
        SetRayTypeText();
    }
    
}
