using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The PhysicsProjectile class is a Projectile fired by the PhysicsRay to effect <see cref="PhysicsObject">PhysicsObjects</see>.
/// </summary>
public class PhysicsProjectile : Projectile
{
    #region Public fields
    /// <summary>
    /// The RayType of this PhysicsProjectile that determines how it will effect a PhysicsObject.
    /// </summary>
    [HideInInspector]
    public PhysicsRay.RayType RayType;
    #endregion

    protected override void Start()
    {
        base.Start();
        SetColor();
    }

    // Pass of tne RayType effect if colliding with a box or the "no bounce" part of a box
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.GetContact(0).collider.tag == "Box")
        {
            PhysicsObject physicsObject = other.GetContact(0).collider.GetComponent<PhysicsObject>();
            physicsObject.AlterPhysics(RayType);
            Destroy(gameObject);
        }
        else if (other.GetContact(0).collider.tag == "NoBounce")
        {
            PhysicsObject physicsObject = other.GetContact(0).collider.transform.parent.GetComponent<PhysicsObject>();
            physicsObject.AlterPhysics(RayType);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // sets the color based on its RayType
    void SetColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = new Color();
        switch(RayType)
        {
            case PhysicsRay.RayType.Light:
            {
                color = Color.cyan;
                break;
            }

            case PhysicsRay.RayType.Heavy:
            {
                color = Color.red;
                break;
            }

            case PhysicsRay.RayType.Bouncy:
            {
                color = Color.green;
                break;
            }

            case PhysicsRay.RayType.Float:
            {
                color = Color.yellow;
                break;
            }
        }
        sr.color = color;
        transform.GetChild(0).GetComponent<Light2D>().color = color;
    }
}
