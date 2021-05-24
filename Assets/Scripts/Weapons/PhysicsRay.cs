using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicsRay : Weapon
{

    #region Public enum
    public enum RayType
    {
        Light,
        Heavy,
        Bouncy,
        Float
    }
    #endregion

    #region Type allowed class
    [System.Serializable]
    private class TypeAllowed
    {
        public RayType type;
        public bool allowed;

        public TypeAllowed(RayType type, bool allowed)
        {
            this.type = type;
            this.allowed = allowed;
        }

        public static TypeAllowed[] GenerateAllowedTypes(RayType[] allTypes)
        {
            TypeAllowed[] types = new TypeAllowed[allTypes.Length - 1];

            for(int i = 0; i < types.Length; i++)
            {
                types[i] = new TypeAllowed(allTypes[i + 1], false);
            }

            return types;
        }
    }
    #endregion

    #region Inspector fields   
    [SerializeField] TypeAllowed[] allowedTypes = new TypeAllowed[]
    {
        new TypeAllowed(RayType.Light, true),
        new TypeAllowed(RayType.Heavy, true),
        new TypeAllowed(RayType.Bouncy, true),
        new TypeAllowed(RayType.Float, true)
    };
    #endregion

    #region Public fields
    public RayType CurrentRay
    {
        get {return currentRayType;}
    }
    #endregion

    #region Private fields
    private RayType currentRayType;
    private int index;

    private int recordingStartIndex = -1;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        NextRayType();
        recordingStartIndex = index;

        pickUpPoint = new Vector3(0.963f, 0.069f, 0);
    }

    public override GameObject Shoot(Quaternion rotation)
    {
        if(accumulatedTime >= fireCooldown)
        {
            GameObject go = Instantiate(projectile, transform.GetChild(0).position, rotation);
            go.layer = 9;
            PhysicsProjectile p = go.GetComponent<PhysicsProjectile>();
            p.direction = transform.parent.GetChild(1).position - transform.parent.position;
            p.SetShooter(transform.parent.parent.gameObject);
            p.rayType = currentRayType;
            accumulatedTime = 0;
            return go;
        }
        return null;
    }

    public void NextRayType()
    {
        if(index < allowedTypes.Length - 1)
        {
            index++;
            while(!allowedTypes[index].allowed)
            {
                index++;
                if(index >= allowedTypes.Length) index = 0;
            }
            currentRayType = allowedTypes[index].type;
        }
        else
        {
            index = 0;
            while(!allowedTypes[index].allowed)
            {
                index++;
                if(index >= allowedTypes.Length) index = 0;
            }
            currentRayType = allowedTypes[index].type;
        }
    }

    public void PrevRayType()
    {
        if(index > 0)
        {
            index--;
            while(!allowedTypes[index].allowed)
            {
                index--;
                if(index < 0) index = allowedTypes.Length - 1;
            }
            currentRayType = allowedTypes[index].type;
        }
        else
        {
            index = allowedTypes.Length - 1;
            while(!allowedTypes[index].allowed)
            {
                index--;
                if(index < 0) index = allowedTypes.Length - 1;
            }
            currentRayType = allowedTypes[index].type;
        }
    }

    public void SetUIText(TextMeshProUGUI text)
    {
       text.text = currentRayType.ToString();
    }

    public override void ResetWeapon()
    {
        base.ResetWeapon();
        ResetRayType();
    }

    public void ResetRayType()
    {
        index = recordingStartIndex;
        currentRayType = allowedTypes[index].type;
    }

    public void SetStartingType()
    {
        recordingStartIndex = index;
    }

    public void SetRayType(RayType rayType)
    {   
        bool indexFound = false;
        currentRayType = rayType;
        for(int i = 0; (i < allowedTypes.Length && !indexFound); i++)
        {
            if(rayType == allowedTypes[i].type) 
            {
                index = i;
                indexFound = true;
            }
        }    
    }
}
