using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The PhysicsObject class represents objects that can be altered by the PhysicsRay.
/// </summary>
public class PhysicsObject : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The mass of this object when it's under the \"Light\" physics effect.")]
    [SerializeField] private float _lightMass = 50;
    [Tooltip("The mass of this object when it's under the \"Heavy\" physics effect.")]
    [SerializeField] private float _heavyMass = 300;
    [Tooltip("The PhysicsMaterial that causes the object to bounce.")]
    [SerializeField] private PhysicsMaterial2D _bounceMaterial;
    [Tooltip("The inital speed of the bounce when it launches from the ground.")]
    [SerializeField] private float _initialBounceSpeed = 10;
    [Tooltip("How high the object can float.")]
    [SerializeField] private float _floatHeight = 10;
    [Tooltip("How fast the object is when floating upwards.")]
    [SerializeField] private float _floatSpeed = 2;

    #endregion

    #region  Private fields
    private static List<PhysicsObject> _allPhysicsObjects;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private float _initialMass;
    private Vector3 _startingPosition;
    private PhysicsMaterial2D _initialPhysicsMaterial;
    private float _yPosOnGround;
    private float _yPosInAir;
    private bool _touchingCeiling;
    private Coroutine _floatRoutine;
    private Light2D _light;
    #endregion

    void Awake()
    {
        if (_allPhysicsObjects == null) _allPhysicsObjects = new List<PhysicsObject>();
    }

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _initialMass = _rigidbody2D.mass;
        _startingPosition = transform.position;
        _initialPhysicsMaterial = _rigidbody2D.sharedMaterial;
        _yPosOnGround = transform.position.y;
        _allPhysicsObjects.Add(this);
        _light = transform.GetChild(0).GetComponent<Light2D>();
        _light.pointLightOuterRadius *= transform.localScale.x;
    }

    void Update()
    {
        // Stop the box from moving up or down after the float routine has ended
        if (_rigidbody2D.gravityScale == 0 && _floatRoutine == null)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        }
    }

    /// <summary>
    /// Updates the starting position.
    /// </summary>
    public void UpdateStartingPosition()
    {
        _startingPosition = transform.position;
    }

    // Resets the properites of the object back to its default state
    void ResetPhysics(bool resetVelocity)
    {
        if(resetVelocity) _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.isKinematic = false;
        _rigidbody2D.mass = _initialMass;
        _rigidbody2D.sharedMaterial = _initialPhysicsMaterial;
        _rigidbody2D.gravityScale = 1;
        _rigidbody2D.drag = 0;
        if (_floatRoutine != null) StopCoroutine(_floatRoutine);

        _spriteRenderer.color = Color.white;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls <see cref="UpdateStartingPosition"/> on all of the <see cref="PhysicsObject">PhysicsObjects</see> in the scene.
    /// </summary>
    public static void UpdateAllInitialPositions()
    {
        if (_allPhysicsObjects != null)
        {
            foreach (PhysicsObject physicsObject in _allPhysicsObjects)
            {
                physicsObject.UpdateStartingPosition();
            }
        }
    }

    /// <summary>
    /// Resets the properties of every PhysicsObject in the scene back to their default state.
    /// </summary>
    /// <param name="resetPosition">Whether or not the position should be reset as well.</param>
    /// <param name="resetVelocity">Whether or not the velocity should be reset to zero as well.</param>
    public static void ResetAllPhysics(bool resetPosition, bool resetVelocity)
    {
        if (_allPhysicsObjects != null)
        {
            foreach (PhysicsObject physicsObject in _allPhysicsObjects)
            {
                physicsObject.ResetPhysics(resetVelocity);
                if (resetPosition) 
                {
                    physicsObject.transform.position = physicsObject._startingPosition;
                    physicsObject._yPosOnGround = physicsObject.transform.position.y;
                }
            }
        }
    }

    void MakeLight()
    {
        _rigidbody2D.mass = _lightMass;
        _spriteRenderer.color = Color.cyan;
    }

    void MakeHeavy()
    {
        _rigidbody2D.mass = _heavyMass;
        _spriteRenderer.color = Color.red;
    }

    void MakeBouncy()
    {
        _rigidbody2D.sharedMaterial = _bounceMaterial;
        _rigidbody2D.drag = 0;

        //Starting bounce if the object was stationary
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 1 &&
            Mathf.Abs(transform.position.y - _yPosOnGround) < 0.1f)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _initialBounceSpeed);
        }

        _spriteRenderer.color = Color.green;
    }

    void MakeFloat()
    {
        _yPosInAir = _yPosOnGround + _floatHeight;
        _floatRoutine = StartCoroutine(GoUp());
        _rigidbody2D.gravityScale = 0;
        _spriteRenderer.color = Color.yellow;
    }

    IEnumerator GoUp()
    {
        while (transform.position.y < _yPosInAir)
        {
            _rigidbody2D.velocity = new Vector2(0, _floatSpeed);
            yield return null;
        }
        _floatRoutine = null;
    }

    /// <summary>
    /// Alters the physical properties of this PhysicsObject to make it Light, Heavy, Bouncy or Float.
    /// </summary>
    /// <remarks>
    /// Also resets the physics of every other PhysicsObject in the scene without reseting their position or velocity.
    /// </remarks>
    /// <seealso cref="PhysicsRay"/>
    /// <seealso cref="ResetAllPhysics(bool, bool)"/>
    /// <param name="rayType">The type of alteration that is to be made to this object.</param>
    public void AlterPhysics(PhysicsRay.RayType rayType)
    {
        ResetAllPhysics(false, false);
        switch (rayType)
        {
            case PhysicsRay.RayType.Light:
                {
                    MakeLight();
                    break;
                }

            case PhysicsRay.RayType.Heavy:
                {
                    MakeHeavy();
                    break;
                }

            case PhysicsRay.RayType.Bouncy:
                {
                    MakeBouncy();
                    break;
                }

            case PhysicsRay.RayType.Float:
                {
                    MakeFloat();
                    break;
                }
        }
        _light.gameObject.SetActive(true);
        _light.color = _spriteRenderer.color;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);

        // Resets the last ground position to adjust the height this object can float
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.normal == Vector2.up && _rigidbody2D.gravityScale > 0)
            {
                _yPosOnGround = transform.position.y;
            }
        }
    }
    void OnDestroy()
    {
        if(_allPhysicsObjects != null) _allPhysicsObjects.Remove(this);
    }
}
