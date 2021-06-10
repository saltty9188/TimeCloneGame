using UnityEngine;

/// <summary>
/// The GuardBot class controls the actions of GuardBot enemies.
/// </summary>
public class GuardBot : EnemyBehaviour
{

    #region Inspector fields
    [Tooltip("The radius that the enemy will search for targets in.")]
    [SerializeField] private float _aimRadius = 10;
    [Tooltip("The length of ground that the enemy will patrol.")]
    [SerializeField] private float _moveBoundary = 20;
    [Tooltip("How fast the enemy moves.")]
    [SerializeField] private float _moveSpeed = 2;
    [Tooltip("The amount of damage the enemy does.")]
    [SerializeField] private int _damage = 5;
    [Tooltip("The projectile prefab the enemy will fire.")]
    [SerializeField] private GameObject _projectilePrefab;
    [Tooltip("What can the enemy aim through?")]
    [SerializeField] private LayerMask _aimBlockers;
    [Tooltip("The exit to the room this enemy occupies.")]
    [SerializeField] private Door _roomExit;
    #endregion
    
    #region Private fields
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private EnemyStatus _enemyStatus;
    private float _leftBoundary;
    private float _rightBoundary;
    private GameObject _currentTarget;
    private bool _closestTargetIsClone;
    private float _startSpeed;
    private bool _collisionBelow = false;
    private bool _collisionAbove = false;
    private Rigidbody2D _aboveContact = null;
    private Rigidbody2D _belowContact = null;
    #endregion

    
    protected override void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _enemyStatus = GetComponent<EnemyStatus>();
        _leftBoundary = transform.position.x - _moveBoundary / 2f;
        _rightBoundary = transform.position.x + _moveBoundary / 2f;
        _currentTarget = null;
        _startSpeed = _moveSpeed;
        EnemyManager.Enemies.Add(gameObject);
    }

    
    void Update()
    {
        // Only patrol and look for a target if one is not found or the current target is not a clone
        if(_currentTarget == null || !_closestTargetIsClone)
        {
            if(_currentTarget == null) Patrol();
            AcquireTarget();
        }

        if(_currentTarget != null)
        {
            bool hitTarget = false;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _currentTarget.transform.position - transform.position, _aimRadius, _aimBlockers);
            if(hits != null)
            {
                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.tag == "Player" || hit.collider.tag == "Clone")
                    {
                        hitTarget = true;
                    }
                }
            }

            if(hitTarget)
            {
                //shooting animation
                _animator.SetBool("EnemyNearby", true);
                MoveTowardTarget();
            }
            else
            {
                _animator.SetBool("EnemyNearby", false);
                _currentTarget = null;
            }
        }

        //Get crushed
        if(_collisionAbove && _collisionBelow)
        {
            gameObject.SetActive(false);
        }
    }

    void AcquireTarget()
    {
        float closestDist = float.MaxValue;
        GameObject closestTarget = null;
        _closestTargetIsClone = false;
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
                //Prefer clones
                if(target.tag == "Clone" && !_closestTargetIsClone && dist < _aimRadius)
                {
                    _closestTargetIsClone = true;
                    closestDist = dist;
                    closestTarget = target;
                } 
                else if(dist < closestDist && dist <= _aimRadius)
                {
                    // Update the closest target if a clone was not found or a new clone is closer
                    if(!_closestTargetIsClone && target.tag == "Player")
                    {
                        closestDist = dist;
                        closestTarget = target;
                    }
                    else if(_closestTargetIsClone && target.tag == "Clone")
                    {
                        closestDist = dist;
                        closestTarget = target;
                    }
                }
            }
        }
        _currentTarget = closestTarget;
    }

    void Patrol()
    {
        // Change direction and flip at each boundary
        _rigidbody2D.velocity = new Vector2(_moveSpeed, 0);
        if((transform.position.x >= _rightBoundary || AtEdge(true)) && _moveSpeed > 0)
        {
            _moveSpeed *= -1;
            Flip();
        }

        if((transform.position.x <= _leftBoundary || AtEdge(false)) && _moveSpeed < 0)
        {
            _moveSpeed *= -1;
            Flip();
        }
    }

    void MoveTowardTarget()
    {
        Vector2 spriteSize = GetComponent<SpriteRenderer>().bounds.size;

        // Only flip if the target is on the other side of the enemy sprite
        if(_currentTarget.transform.position.x <= transform.position.x - spriteSize.x / 2f)
        {
            if(_moveSpeed > 0)
            {
                _moveSpeed *= -1;
                Flip();
            }
        }
        else if(_currentTarget.transform.position.x >= transform.position.x + spriteSize.x / 2f)
        {
            if(_moveSpeed < 0)
            {
                _moveSpeed *= -1;
                Flip();
            }
        }

        // Don't move if the enemy is at an edge
        if(AtEdge(_moveSpeed > 0))
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            _rigidbody2D.velocity = new Vector2(_moveSpeed, 0);
        }
    }

    // Tests if the enemy is at an edge
    bool AtEdge(bool rightSide)
    {
        Vector2 spriteSize = GetComponent<SpriteRenderer>().bounds.size;

        Vector2 raycastPoint = new Vector2();
        if(rightSide)
        {
            raycastPoint = new Vector2(transform.position.x + spriteSize.x / 2f, transform.position.y - spriteSize.y / 2f);
        }
        else
        {
            raycastPoint = new Vector2(transform.position.x - spriteSize.x / 2f, transform.position.y - spriteSize.y / 2f);
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastPoint, Vector2.down, 0.1f);
        if(hits != null)
        {
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.tag != tag)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Animation event that fires a bullet towards the current target.
    /// </summary>
    public void Shoot()
    {
        if(_currentTarget != null)
        {
            Vector2 direction = _currentTarget.transform.position - transform.position;
            GameObject go = Instantiate(_projectilePrefab, transform.GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.Redirect(direction);
            p.Damage = _damage;
            p.SetShooter(gameObject);   

            AudioManager.Instance.PlaySFX("GunShot");
        }  
    }

    void Flip()
    {
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    /// <summary>
    /// Caches the enemy's current position, scale, starting speed, and active state
    /// </summary>
    public override void CacheInfo()
    {
        base.CacheInfo();
        _startSpeed = _moveSpeed;
    }

    /// <summary>
    /// Resets the enemy to its cached position, scale, speed, and active state.
    /// </summary>
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        _moveSpeed = _startSpeed;
        _closestTargetIsClone = false;
        if(_roomExit) _roomExit.RemoveActivation();
    }

    /// <summary>
    /// Calls <see cref="EnemyStatus.TakeDamage(int, Vector2)">TakeDamage</see> and flips if there is no current target.
    /// </summary>
    /// <param name="damage">The damage to be taken.</param>
    public void TakeDamage(int damage)
    {
        if(_currentTarget == null)
        {
            _moveSpeed *= -1;
            Flip();
        }

        _enemyStatus.TakeDamage(damage);
    }

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.collider.tag != "Player" && contact.collider.tag != "Clone")
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if(angle < 0.5f)
                {
                    _collisionBelow = true;
                    _belowContact = contact.collider.GetComponent<Rigidbody2D>();
                }
                else if(angle > 179.5f)
                {
                    _collisionAbove = true;
                    _aboveContact = contact.collider.GetComponent<Rigidbody2D>();
                }
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

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aimRadius);
    }

    void OnDisable() 
    {
        // Activate the room exit upon death
        if(_roomExit) _roomExit.AddActivation();    
    }
}
