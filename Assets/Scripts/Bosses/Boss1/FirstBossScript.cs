using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FirstBossScript class is the main class that drives the actions of the First Boss.
/// </summary>
public class FirstBossScript : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Prefab for the bullets the boss fires.")]
    [SerializeField] private GameObject _bulletPrefab;
    [Tooltip("The damage the bullets will do.")]
    [SerializeField] private int _damage = 5;
    [Tooltip("The amount of time in seconds before the boss moves.")]
    [SerializeField] private float _phaseTransitionTime = 10;
    [Tooltip("Array of sprites for the death pieces.")]
    [SerializeField] private Sprite[] _deathPieces;
    [Tooltip("Prefab for the objects that spawn upon the boss's death.")]
    [SerializeField] private GameObject _deathPiecePrefab;
    #endregion

    #region Public fields
    /// <value>The initial position of the boss at the start of the fight.</value>
    [HideInInspector]
    public Vector3 InitialPosition
    {
        get {return _initialPosition;}
    }
    #endregion

    #region Private fields
    private Animator _animator;
    private GameObject _closestTarget;
    private float _shootTime;
    private bool _inFight;
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    #endregion

    void Start()
    {
        // Initalise values
        _initialPosition = transform.position;
        _initialScale = transform.localScale;

        _animator = GetComponent<Animator>();
        _shootTime = 0;
    }

    void Update()
    {
        SelectTarget();

        // If the boss isn't moving then it must be shooting
        bool shooting = !_animator.GetBool("Moving");

        if(shooting && _inFight)
        {
            _shootTime += Time.deltaTime;
            // Switch sides 
            if(_shootTime >= _phaseTransitionTime)
            {
                _animator.SetBool("Moving", true);
                _shootTime = 0;
            }
        }

        // Failsafe to ensure the boss is in the correct position upon resetting
        if(!_inFight && transform.position != InitialPosition)
        {
            transform.position = InitialPosition;
        }
    }

    /// <summary>
    /// Animation event that fires a bullet prefab out of each shoulder launcher on the boss.
    /// </summary>
    public void Shoot()
    {
        if(_closestTarget != null)
        {
            //Shoulder launcher 1
            Vector2 direction = _closestTarget.transform.position - transform.position;
            GameObject go = Instantiate(_bulletPrefab, transform.GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.noHitTime = 0.4f;
            p.damage = _damage;
            p.direction = direction;
            p.transform.localScale = new Vector3(2, 2, 2);
            p.IgnoreCollision(transform.GetChild(2).gameObject);
            p.SetShooter(gameObject);

            //Shoulder launcher 2
            go = Instantiate(_bulletPrefab, transform.GetChild(1).position, new Quaternion());
            p = go.GetComponent<Projectile>();
            p.noHitTime = 0.4f;
            p.damage = _damage;
            p.direction = direction;
            p.transform.localScale = new Vector3(2, 2, 2);
            p.IgnoreCollision(transform.GetChild(2).gameObject);
            p.SetShooter(gameObject);

            AudioManager.Instance.PlaySFX("GunShot", 0.7f);
        }   
    }

    /// <summary>
    /// Trigger that causes the boss fight to begin.
    /// </summary>
    public void StartFight()
    {
        _animator.SetTrigger("Start Fight");
        _animator.ResetTrigger("Reset");
        _inFight = true;
    }

    /// <summary>
    /// Resets the values for the boss and makes it ready to begin again.
    /// </summary>
    public void ResetBoss()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = InitialPosition;
        transform.localScale = _initialScale;
        _shootTime = 0;
        _inFight = false;

        GetComponent<BossStatus>().ResetStatus();

        _animator.ResetTrigger("Start Fight");
        _animator.SetBool("OnRight", true);
        _animator.SetBool("Moving", false);
        
        _animator.SetTrigger("Reset");
        
    }

    void SelectTarget()
    {
        float closestDist = float.MaxValue;
        for(int i = 0; i < EnemyManager.Targets.Count; i++)
        {
            if(EnemyManager.Targets[i] == null)
            {
                EnemyManager.Targets.RemoveAt(i);
                i--;
            }
            else
            {
                GameObject target = EnemyManager.Targets[i];
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if(dist < closestDist)
                {
                    closestDist = dist;
                    _closestTarget = target;
                }
            }     
        }
    }

    /// <summary>
    /// Spawns some broken pieces of the Boss upon its death.
    /// </summary>
    public void DeathPieces()
    {
        foreach(Sprite deathSprite in _deathPieces)
        {
            Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.value, 0).normalized;
            GameObject piece = Instantiate(_deathPiecePrefab, transform.position + direction, new Quaternion());
            piece.GetComponent<SpriteRenderer>().sprite = deathSprite;
            
            Physics2D.IgnoreCollision(piece.GetComponent<Collider2D>(), GameObject.Find("Player").GetComponent<Collider2D>());

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * 2000);
        }
    }
}
