using UnityEngine;
using TMPro;

/// <summary>
/// The PhysicsRay class is a type of Weapon that fires <see cref="PhysicsProjectile">PhysicsProjectiles</see> to effect <see cref="PhysicsObject">PhysicsObjects</see>.
/// </summary>
public class PhysicsRay : Weapon
{

    #region Public enum
    /// <summary>
    /// The RayType enum is used for deteriming the type of change to be applied to a PhysicsObject.
    /// </summary>
    public enum RayType
    {
        Light,
        Heavy,
        Bouncy,
        Float
    }
    #endregion

    #region Type allowed class
    // small class that contains a raytype and a boolean to determine if that type is available or not
    // would be a struct but unity doesn't support them in the inspector
    [System.Serializable]
    private class TypeAllowed
    {
        public RayType Type;
        public bool Allowed;
        
        public TypeAllowed(RayType type, bool allowed)
        {
            this.Type = type;
            this.Allowed = allowed;
        }
    }
    #endregion

    #region Inspector fields   
    [SerializeField] private TypeAllowed[] _allowedTypes = new TypeAllowed[]
    {
        new TypeAllowed(RayType.Light, true),
        new TypeAllowed(RayType.Heavy, true),
        new TypeAllowed(RayType.Bouncy, true),
        new TypeAllowed(RayType.Float, true)
    };
    #endregion

    #region Public fields
    /// <value>The currently selected RayType of this PhysicsRay.</value>
    public RayType CurrentRay
    {
        get {return _currentRayType;}
    }
    #endregion

    #region Private fields
    private RayType _currentRayType;
    private int _index;
    private int _recordingStartIndex = -1;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        _recordingStartIndex = _index;

        PickUpPoint = new Vector3(0.963f, 0.069f, 0);
    }

    /// <summary>
    /// Shoots a PhysicsProjectile in a direction given by the rotation.
    /// </summary>
    /// <remarks>
    /// The RayType of the PhysicsProjectile is the same as the <see cref="CurrentRay"/>.
    /// </remarks>
    /// <seealso cref="PlayerStatus.AddProjectile(GameObject)"/>
    /// <param name="rotation">The rotation of the parent object.</param>
    /// <returns>The fired PhysicsProjectile to be added to the projectile parent.</returns>
    public override GameObject Shoot(Quaternion rotation)
    {
        if(AccumulatedTime >= FireCooldown)
        {
            GameObject go = Instantiate(Projectile, transform.GetChild(0).position, rotation);
            go.layer = 9;
            PhysicsProjectile p = go.GetComponent<PhysicsProjectile>();
            p.Redirect(transform.parent.GetChild(0).position - transform.parent.position);
            p.SetShooter(transform.parent.parent.gameObject);
            p.RayType = _currentRayType;
            AccumulatedTime = 0;
            AudioManager.Instance.PlaySFX("PhysicsRay");
            return go;
        }
        return null;
    }

    /// <summary>
    /// Cycles to the next allowed RayType.
    /// </summary>
    public void NextRayType()
    {
        if(_index < _allowedTypes.Length - 1)
        {
            _index++;
            while(!_allowedTypes[_index].Allowed)
            {
                _index++;
                if(_index >= _allowedTypes.Length) _index = 0;
            }
            _currentRayType = _allowedTypes[_index].Type;
        }
        else
        {
            _index = 0;
            while(!_allowedTypes[_index].Allowed)
            {
                _index++;
                if(_index >= _allowedTypes.Length) _index = 0;
            }
            _currentRayType = _allowedTypes[_index].Type;
        }
    }

    /// <summary>
    /// Cycles to the previous allowed RayType.
    /// </summary>
    public void PrevRayType()
    {
        if(_index > 0)
        {
            _index--;
            while(!_allowedTypes[_index].Allowed)
            {
                _index--;
                if(_index < 0) _index = _allowedTypes.Length - 1;
            }
            _currentRayType = _allowedTypes[_index].Type;
        }
        else
        {
            _index = _allowedTypes.Length - 1;
            while(!_allowedTypes[_index].Allowed)
            {
                _index--;
                if(_index < 0) _index = _allowedTypes.Length - 1;
            }
            _currentRayType = _allowedTypes[_index].Type;
        }
    }

    /// <summary>
    /// Sets the given text component to be the current RayType.
    /// </summary>
    /// <param name="text">The text component to change.</param>
    public void SetUIText(TextMeshProUGUI text)
    {
       text.text = _currentRayType.ToString();
    }

    /// <summary>
    /// Resets the position of the PhysicsRay and restores it to its starting RayType.
    /// </summary>
    /// <seealso cref="ResetRayType"/>
    public override void ResetWeapon()
    {
        base.ResetWeapon();
        ResetRayType();
    }

    /// <summary>
    /// Resets the PhysicsRay to its starting RayType.
    /// </summary>
    public void ResetRayType()
    {
        _index = _recordingStartIndex;
        _currentRayType = _allowedTypes[_index].Type;
    }

    /// <summary>
    /// Sets the starting RayType to the current RayType.
    /// </summary>
    public void SetStartingType()
    {
        _recordingStartIndex = _index;
    }

    /// <summary>
    /// Sets the RayType of this PhysicsRay.
    /// </summary>
    /// <param name="rayType">The RayType this PhysicsRay is to be changed to.</param>
    public void SetRayType(RayType rayType)
    {   
        bool indexFound = false;
        _currentRayType = rayType;
        for(int i = 0; (i < _allowedTypes.Length && !indexFound); i++)
        {
            if(rayType == _allowedTypes[i].Type) 
            {
                _index = i;
                indexFound = true;
            }
        }    
    }
}
