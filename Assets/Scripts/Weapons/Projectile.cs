using UnityEngine;

/// <summary>
/// The Projectile class is used to deal damage to enemies after being fired from a Weapon.
/// </summary>
public class Projectile : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("How fast will this projectile travel.")]
    [SerializeField] private float _speed;
    /// <summary>
    /// The amount of damage this Projectile will do.
    /// </summary>
    [Tooltip("The amount of damage this projectile will do.")]
    [SerializeField] public int Damage;
    /// <summary>
    /// The amount of time that must pass before the Projectile can damage the one who shot it.
    /// </summary>
    [Tooltip("The amount of time that must pass before the projectile can damage the one who shot it.")]
    [SerializeField] public float NoHitTime = 0.1f;
    /// <summary>
    /// Whether this Projectile is a laser or not.
    /// </summary>
    [Tooltip("Whether this projectile is a laser or not.")]
    [SerializeField] public bool Laser;
    #endregion

    #region Private fields
    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rigidbody2D;
    private GameObject _shooter;
    private Vector2 _direction;
    private float _aliveTime;
    private bool _pastShooter;
    private bool _ignoringShooter;
    private float _reflectionCooldown;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        _direction = new Vector2(1, 0).normalized;
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _aliveTime = 0;
        _pastShooter = false;
        _ignoringShooter = false;
        _reflectionCooldown = 0;
    }

    protected virtual void Start()
    {
        Redirect(_direction);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_aliveTime < NoHitTime) _aliveTime += Time.deltaTime;
        else 
        {
            if(!_pastShooter && !_ignoringShooter)
            {
                IgnoreShooterCollision(false);
                _pastShooter = true;
            }
        }

        if(_reflectionCooldown > 0) _reflectionCooldown -= Time.deltaTime;

        if(Camera.main)
        {
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

            //Destroy projectile if its off camera
            if (screenPos.x < -0.5f || screenPos.x > 1.5f || screenPos.y < -0.5f || screenPos.y > 1.5f)
            {
                GameObject.Destroy(gameObject);
            }
        }

        //Catch collision bugs
        if (_rigidbody2D.velocity == Vector2.zero) Destroy(gameObject);
    }

    // handle collisions with various entities
    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D collider = other.GetContact(0).collider;
        string colliderTag = collider.tag;

        switch(colliderTag)
        {
            //evaluate player collision first
            case "Clone":
            case "Player":
            {
                collider.GetComponent<PlayerStatus>().TakeDamage(Damage);  
                break;
            }
            case "Target":
            {
                Target t = collider.GetComponent<Target>();
                if (t)
                {
                    t.Activate();
                }
                break;
            }
            case "WeakPoint":
            {
                if(collider.transform.parent)
                {
                    WeakPointCollision(collider.transform.parent.tag, collider.transform.parent.gameObject);
                }
                break;
            }
            case "Boss2":
            {
                BossStatus sbs = collider.GetComponent<BossStatus>();
                if(sbs)
                {
                    sbs.TakeDamage(Damage);
                }
                break;
            }
            case "Boss3Back":
            {
                ThirdBossScript tbs = collider.transform.parent.GetComponent<ThirdBossScript>();
                if(tbs)
                {
                    tbs.MakeVulnerable();
                }
                break;
            }
            case "Enemy":
            {
                EnemyStatus es = collider.GetComponent<EnemyStatus>();
                if(es)
                {
                    es.TakeDamage(Damage, -other.GetContact(0).normal);
                }
                break;
            }
            case "Reflective":
            {
                if(Laser)
                {
                    //reflect
                    Redirect(Vector2.Reflect(_direction, other.contacts[0].normal));
                    _reflectionCooldown = 0.1f;
                    return;
                }
                break;
            }
        }

        Destroy(gameObject);
    }

    // used if second collision was inbetween physics ticks
    void OnCollisionStay2D(Collision2D other)
    {
        if (Laser && other.GetContact(0).collider.tag == "Reflective" && _reflectionCooldown <= 0)
        {
            //ricochete
            Redirect(Vector2.Reflect(_direction, other.contacts[0].normal));
            _reflectionCooldown = 0.1f;
        }
    }

    // whether or not to ignore collision with the shooter
    void IgnoreShooterCollision(bool ignore)
    {
        if (_shooter != null)
        {
            Collider2D[] colliders = _shooter.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), ignore);
            }
        }
    }

    /// <summary>
    /// Ignores collisions with the given GameObject.
    /// </summary>
    /// <param name="objectToIgnore">The GameObject to ignore.</param>
    public void IgnoreCollision(GameObject objectToIgnore)
    {
        Physics2D.IgnoreCollision(objectToIgnore.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());
    }

    /// <summary>
    /// Sets the GameObject who fired this Projectile so collisions with them can be ignored when the Projectile is first fired.
    /// </summary>
    /// <param name="shooter">The GameObject that shot this Projectile.</param>
    /// <param name="ignoreShooter">Whether or not to ignore the shooter permanently.</param>
    public void SetShooter(GameObject shooter, bool ignoreShooter = false)
    {
        this._shooter = shooter;
        this._ignoringShooter = ignoreShooter;
        IgnoreShooterCollision(true);
    }

    /// <summary>
    /// Redirects this Projectile in the given direction.
    /// </summary>
    /// <param name="newDirection">The direction for this Projectile to travel.</param>
    public void Redirect(Vector2 newDirection)
    {
        _direction = newDirection.normalized;
        float newRotation = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, newRotation);

        _rigidbody2D.velocity = _direction * _speed;
    }

    // collision cases with a weakpoint
    void WeakPointCollision(string enemyParentTag, GameObject parent)
    {
        switch(enemyParentTag)
        {
            case "Boss1":
            {
                BossStatus bs = parent.GetComponent<BossStatus>();
                if (bs)
                {
                    bs.TakeDamage(Damage);
                }
                break;
            }
            case "Boss3":
            {
                ThirdBossScript tbs = parent.GetComponent<ThirdBossScript>();
                if(tbs)
                {
                    tbs.GetHit(Damage);
                }
                break;
            }
            case "Boss3Base":
            {
                ThirdBossScript tbs = parent.GetComponentInChildren<ThirdBossScript>();
                if(tbs)
                {
                    tbs.GetHit(Damage);
                }
                break;
            }
            case "EnemyInvuln":
            {
                GuardBot gb = parent.GetComponent<GuardBot>();
                if(gb && parent != _shooter)
                {
                    gb.TakeDamage(Damage);
                }
                break;
            }
        }
    }
}
