using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PhysicsProjectile : Projectile
{
    #region Public fields
    public PhysicsRay.RayType rayType;
    #endregion

    protected override void Start()
    {
        base.Start();
        SetColor();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.GetContact(0).collider.tag == "Box")
        {
            PhysicsObject physicsObject = other.GetContact(0).collider.GetComponent<PhysicsObject>();
            physicsObject.AlterPhysics(rayType);
            Destroy(gameObject);
        }
        else if (other.GetContact(0).collider.tag == "NoBounce")
        {
            PhysicsObject physicsObject = other.GetContact(0).collider.transform.parent.GetComponent<PhysicsObject>();
            physicsObject.AlterPhysics(rayType);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = new Color();
        switch(rayType)
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
