using UnityEngine;

/// <summary>
/// The Turret class is responsible for controlling Turret enemy behaviour.
/// </summary>
public class Turret : EnemyBehaviour
{
    #region Inspector fields
    [Tooltip("Does this turret have a base?")]
    [SerializeField] private bool _hasBase = false;
    [Tooltip("The radius that the turret will search for targets in.")]
    [SerializeField] private float _aimRadius = 10;
    [Tooltip("The projectile prefab the turret will fire.")]
    [SerializeField] private GameObject _projectilePrefab;
    [Tooltip("The amount of team between gun shots.")]
    [SerializeField] private float _fireCooldown = 0.3f;
    [Tooltip("The amount of damage the turret does.")]
    [SerializeField] private int _damage = 10;
    [Tooltip("The number of explosions that will spawn upon this turret's destruction.")]
    [SerializeField] private int _numExplosions = 2;
    [Tooltip("Everything except for projectiles and triggers.")]
    [SerializeField] private LayerMask _everythingButBullet;
    #endregion

    #region Public fields
    /// <value>Returns true if this Turret has a base.</value>
    public bool HasBase
    {
        get {return _hasBase;}
    }
    #endregion
    
    #region Private fields
    private float _accumulatedTime;
    private GameObject _currentTarget;
    private bool _collisionBelow = false;
    private bool _collisionAbove = false;
    private Rigidbody2D _aboveContact = null;
    private Rigidbody2D _belowContact = null;
    #endregion

    protected override void Start()
    {
        base.Start();
        EnemyManager.Enemies.Add(gameObject);
    }

    void Update()
    {  
        if(_currentTarget == null) AcquireTarget();

        if(_currentTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentTarget.transform.position - transform.position, _aimRadius, _everythingButBullet);
            if(hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
            {
                FaceTarget(_currentTarget.transform.position);
                Shoot(_currentTarget.transform.position - transform.position);
            }
            else
            {
                _currentTarget = null;
            }
        }

        if(_accumulatedTime < _fireCooldown) _accumulatedTime += Time.deltaTime;

        // Get crushed
        if(_collisionAbove && _collisionBelow)
        {
            if(_hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void AcquireTarget()
    {
        float closestDist = float.MaxValue;
        GameObject closestTarget = null;
        for(int i = 0; i < EnemyManager.Targets.Count; i++)
        {
            // Remove the target from the list if it's been destroyed
            if(EnemyManager.Targets[i] == null)
            {
                EnemyManager.Targets.RemoveAt(i);
                i--;
            }
            else
            {
                GameObject target = EnemyManager.Targets[i];
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if(dist < closestDist && dist <= _aimRadius)
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }
        }
        _currentTarget = closestTarget;
    }

    // Shoots in the given direction
    void Shoot(Vector2 direction)
    {
        if(_accumulatedTime >= _fireCooldown)
        {
            GameObject go = Instantiate(_projectilePrefab, transform.GetChild(0).GetChild(0).GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.Redirect(direction);
            p.Damage = _damage;
            p.SetShooter(gameObject);
            _accumulatedTime = 0;

            if(p.Laser)
            {
                AudioManager.Instance.PlaySFX("LaserFire", 0.7f);
            }
            else
            {
                AudioManager.Instance.PlaySFX("GunShot");
            }
        }
    }

    // Rotates the turret to face the target
    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.collider.tag != "Player" && contact.collider.tag != "Clone" && contact.collider.tag != "Projectile")
            {
                if(contact.point.y < transform.position.y)
                {
                    _collisionBelow = true;
                    _belowContact = contact.collider.GetComponent<Rigidbody2D>();
                }
                else if(contact.point.y > transform.position.y)
                {
                    _collisionAbove = true;
                    _aboveContact = contact.collider.GetComponent<Rigidbody2D>();
                }
            }
        }

        // Get crushed
        if(_collisionAbove && _collisionBelow)
        {
            CreateExplosions();
            if(_hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.point.y < transform.position.y)
            {
                _collisionBelow = true;
                _belowContact = contact.collider.GetComponent<Rigidbody2D>();
            }
            else if(contact.point.y > transform.position.y)
            {
                _collisionAbove = true;
                _aboveContact = contact.collider.GetComponent<Rigidbody2D>();
            }
        }

        // Get crushed
        if(_collisionAbove && _collisionBelow)
        {
            CreateExplosions();
            if(_hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other) 
    {
        if(other.collider.GetComponent<Rigidbody2D>() == _aboveContact)
        {
            _aboveContact = null;
            _collisionAbove = false;
        }
        else if(other.collider.GetComponent<Rigidbody2D>() == _belowContact)
        {
            _belowContact = null;
            _collisionBelow = false;
        }
    }

    /// <summary>
    /// Resets the enemy to its cached position, scale, and active state.
    /// </summary>
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        if(_hasBase)
        {
            transform.parent.gameObject.SetActive(StartActiveState);
        }
    }

    void CreateExplosions()
    {
        float explosionRadius = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4.0f;
        Vector3[] positions = new Vector3[_numExplosions];
        for(int i = 0; i < _numExplosions; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            position.Normalize();
            positions[i] = (position * explosionRadius * Random.Range(0.0f, 1.0f)) + transform.position;
        }

        EnemyManager.Instance.SpawnExplosions(positions);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aimRadius);
    }
}
