using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Aim : MonoBehaviour
{
    #region Inspector fields
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

    #endregion
    
    void Awake()
    {
        facingRight = true;
        angleOffset = 0;
        armRotation = 0;
        SetRayTypeText();
    }

    void Update()
    {
        //Hide back arm when pointing down
        if(transform.rotation.z < -20 * Mathf.Deg2Rad)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public float Rotate(string controlScheme, Vector2 inputDirection, bool shoot)
    {

        if(this.enabled)
            {
                if(controlScheme.Equals("Mouse") || controlScheme.Equals("Keyboard"))
            {
                Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(inputDirection);

                armRotation = Mathf.Atan2(mousePos2D.y - transform.position.y, mousePos2D.x - transform.position.x) * Mathf.Rad2Deg;
            }
            else
            {
                // Leave arm in its last position if player isn't touching the stick
                if(inputDirection != Vector2.zero) armRotation = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            }

            if(Mathf.Abs(armRotation) > 90 && facingRight)
            {
                flip();
                facingRight = false;
                angleOffset = 180;
            }
                
            if(Mathf.Abs(armRotation) < 90 && !facingRight)
            {
                flip();
                facingRight = true;
                angleOffset = 0;
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
        if(Mathf.Abs(armRotation) > 90 && facingRight)
        {
            flip();
            facingRight = false;
            angleOffset = 180;
        }
            
        if(Mathf.Abs(armRotation) < 90 && !facingRight)
        {
            flip();
            facingRight = true;
            angleOffset = 0;
        }
        
        transform.rotation = Quaternion.Euler(0,0, armRotation + angleOffset);

        if(shoot)
        {
            Shoot();
        }
    }

    void flip()
    {
        Vector3 temp = gameObject.transform.parent.localScale;
        temp.x *= -1;
        gameObject.transform.parent.localScale = temp;
    }

    public void PickUpWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        weapon.PickUp(gameObject);
        weapon.held = true;
        SetRayTypeText();
    }

    public void Shoot()
    {
        if(weapon != null)
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
