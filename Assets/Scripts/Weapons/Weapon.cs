using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The Weapon class is used to fire <see cref="Projectile">Projectiles</see>.
/// </summary>
public class Weapon : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The projectile this weapon fires.")]
    [SerializeField] private GameObject _projectile;
    [Tooltip("The amount of time inbetween shots.")]
    [SerializeField] private float _fireCooldown;
    #endregion

    #region Protected fields
    /// <value>The Projectile this weapon fires.</value>
    protected GameObject Projectile
    {
        get {return _projectile;}
    }
    /// <value>The amount of time between shots.</value>
    protected float FireCooldown
    {
        get {return _fireCooldown;}
    }
    /// <summary>
    /// The amount of time that has occured after a shot.
    /// </summary>
    protected float AccumulatedTime;
    /// <summary>
    /// Local position of the Weapon when picked up by the player.
    /// </summary>
    protected Vector3 PickUpPoint;
    #endregion

    #region Private fields
    private SpriteRenderer _spriteRenderer;
    private Aim _aimScript;
    private Light2D _light;
    private Vector3 _initialSpawn;
    private bool _held;
    private bool _justDropped;
    private float _bobValue;
    private float _baseY;
    #endregion

    protected virtual void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        AccumulatedTime = 0;
        _held = false;
        _initialSpawn = transform.position;
        _baseY = _initialSpawn.y;
        _bobValue = 0;
        if(transform.childCount > 1) _light = transform.GetChild(1).GetComponent<Light2D>();
        PickUpPoint = new Vector3(1.428f, 0.138f, 0);
    }

    void Start()
    {
        WeaponManager.Weapons.Add(this);
    }

    void Update()
    {
        // make the weapon bob up and down when not held
        if(!_held)
        {
            transform.position = new Vector3(transform.position.x, _baseY + Mathf.Sin(_bobValue) / 8.0f, transform.position.z);
            _bobValue += Time.deltaTime * 2.0f;
            // Clamp the input angle between 0 and 2PI
            if(_bobValue >= Mathf.PI * 2.0f) _bobValue -= Mathf.PI * 2.0f;
        }

        if(AccumulatedTime < _fireCooldown) AccumulatedTime += Time.deltaTime;

        if(_light && AccumulatedTime >= 0.1f) _light.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the Weapon as being picked up.
    /// </summary>
    /// <param name="holder">The GameObject that picked up this Weapon.</param>
    public void PickUp(GameObject holder)
    {
        transform.SetParent(holder.transform);
        _held = true;
        transform.localPosition = PickUpPoint;
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Drops the Weapon and places it at the given position.
    /// </summary>
    /// <param name="newPos">The new position for this Weapon.</param>
    public void Drop(Vector3 newPos)
    {
        transform.SetParent(null);
        transform.rotation = new Quaternion();
        transform.position = newPos;
        _baseY = transform.position.y;
        _held = false;
        _justDropped = true;
    }

    /// <summary>
    /// Fires this <see cref="Weapon">Weapon's</see> Projectile.
    /// </summary>
    /// <seealso cref="PlayerStatus.AddProjectile(GameObject)"/>
    /// <param name="rotation">The rotation of the parent GameObject.</param>
    /// <returns>The Projectile that was fired to be added to the projectile parent.</returns>
    public virtual GameObject Shoot(Quaternion rotation)
    {
        if(AccumulatedTime >= _fireCooldown)
        {
            GameObject go = Instantiate(_projectile, transform.GetChild(0).position, rotation);
            go.layer = 9;
            Projectile p = go.GetComponent<Projectile>();
            p.Redirect(transform.parent.GetChild(0).position - transform.parent.position);
            p.SetShooter(transform.parent.parent.gameObject);
            AccumulatedTime = 0;

            if(p.Laser)
            {
                AudioManager.Instance.PlaySFX("LaserFire");
            }
            else
            {
                AudioManager.Instance.PlaySFX("GunShot");
            }

            // Muzzle flash for pistol
            if(_light) _light.gameObject.SetActive(true);
            return go;
        }
        return null;
    }

    /// <summary>
    /// Sets the inital position of the Weapon.
    /// </summary>
    public void SetInitalPosition()
    {
        _initialSpawn = transform.position;
        _baseY = _initialSpawn.y;
    }

    /// <summary>
    /// Resets the Weapon to its inital position.
    /// </summary>
    public virtual void ResetWeapon()
    {
        _held = false;
        _justDropped = false;
        transform.parent = null;
        transform.position = _initialSpawn;
        _baseY = _initialSpawn.y;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.rotation = new Quaternion();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(!_held && !_justDropped && other.gameObject.tag == "Player")
        {
            _aimScript = other.gameObject.transform.GetChild(0).GetComponent<Aim>();
            _aimScript.PickUpWeapon(this);
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Player" && !_held)
        {
            _justDropped = false;
        }
    }
}
