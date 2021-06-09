using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The ThirdBossScript class is the main class that drives the actions of the Third Boss.
/// </summary>
public class ThirdBossScript : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Prefab for the bullets the boss fires.")]
    [SerializeField] private GameObject _projectilePrefab;
    [Tooltip("Prefab for the piece of the boss that spawns upon its death.")]
    [SerializeField] private GameObject _deathPrefab;
    [Tooltip("How long the boss is vulnerable after its weakpoint is hit.")]
    [SerializeField] private float _cooldownTime = 0.3f;
    [Tooltip("How long it takes for the vents to switch.")]
    [SerializeField] private float _ventSwapDelay = 20;
    [Tooltip("Ignore projectiles and itself.")]
    [SerializeField] private LayerMask _ignoreSelf;
    [Tooltip("The horizontal door above that blocks a path to the weak point.")]
    [SerializeField] private Door _topDoor;
    [Tooltip("The horizontal door below that blocks a path to the weak point.")]
    [SerializeField] private Door _bottomDoor;
    [Tooltip("The other doors behind the boss that must be closed upon the bosses death.")]
    [SerializeField] private Door[] _otherDoors;
    #endregion

    #region Private fields
    private BossStatus _status;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Animator _frontVent;
    private Animator _topVent;
    private Light2D _shootLight;
    private GameObject _closestTarget;
    private GameObject _projectileParent;
    private bool _inFight;
    private float _hitCooldown;
    private bool _topVulnerable;
    private float _ventSwapCooldown;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _status = GetComponent<BossStatus>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _shootLight = GetComponentInChildren<Light2D>();   

        _topVent =  transform.GetChild(2).GetComponent<Animator>();
        _frontVent = transform.parent.GetChild(1).GetComponent<Animator>();

        InitialValues();

        _projectileParent = new GameObject(name + " Projectiles");
    }

    // Update is called once per frame
    void Update()
    {
        if(_inFight)
        {
            SelectTarget();

            if(_hitCooldown > 0)
            {
                // make the boss tinted blue when it's vulnerable
                _spriteRenderer.color = Color.cyan;
                _hitCooldown -= Time.deltaTime;
            } 
            else
            {
                _spriteRenderer.color = Color.white;
            }

            if(_ventSwapCooldown > 0)
            {
                _ventSwapCooldown -= Time.deltaTime;
            }
            else
            {
                // Swap which vent is open and adjust the doors appropriately
                _ventSwapCooldown = _ventSwapDelay;
                _topVulnerable = !_topVulnerable;
                if(_topVulnerable)
                {
                    _topVent.SetBool("IsOpen", true);
                    _frontVent.SetBool("IsOpen", false);
                    _bottomDoor.AddActivation();
                    _topDoor.RemoveActivation();
                }
                else
                {
                    _topVent.SetBool("IsOpen", false);
                    _frontVent.SetBool("IsOpen", true);
                    _topDoor.AddActivation();
                    _bottomDoor.RemoveActivation();
                }
            }
        }
    }

    /// <summary>
    /// Starts the boss fight.
    /// </summary>
    public void StartFight()
    {
        _inFight = true;
    }

    /// <summary>
    /// Resets the boss fight to its default state.
    /// </summary>
    public void ResetBoss()
    {
        _status.ResetStatus();
        _hitCooldown = 0;
        InitialValues();

        Destroy(_projectileParent);
        _projectileParent = new GameObject(name + " Projectiles");
    }

    void SelectTarget()
    {
        float closestDist = float.MaxValue;
        _closestTarget = null;
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

                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, float.MaxValue, _ignoreSelf);

                if(dist < closestDist && hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
                {
                    closestDist = dist;
                    _closestTarget = target;
                }
            }     
        }

        _animator.SetBool("targetFound", _closestTarget);
    }

    /// <summary>
    /// Animation event that fires a bullet prefab out of the barrel on the boss if there is a target to shoot.
    /// </summary>
    public void Shoot()
    { 
        if(_closestTarget != null)
        {
            Vector2 direction = _closestTarget.transform.position - transform.position;

            GameObject go = Instantiate(_projectilePrefab, transform.GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.Redirect(direction);
            p.transform.localScale = new Vector3(2, 2, 2);
            p.SetShooter(gameObject, true);
            p.transform.parent = _projectileParent.transform;

            AudioManager.Instance.PlaySFX("LaserFire", 0.7f);
        }
    }

    // The inital values for the boss's private fields
    void InitialValues()
    {
        _ventSwapCooldown = _ventSwapDelay;
        _topVulnerable = true;
        _animator.SetBool("targetFound", false);
        _topVent.SetBool("IsOpen", true);
        _frontVent.SetBool("IsOpen", false);

        // By default the bottom door is open and the top door is closed
        _bottomDoor.ResetEvent();
        _bottomDoor.AddActivation();
        _topDoor.ResetEvent();
        _topDoor.RemoveActivation();

        _inFight = false;
        _closestTarget = null;
    }

    /// <summary>
    /// Takes damage if the boss is currently vulnerable.
    /// </summary>
    /// <param name="damage">The damage to be taken.</param>
    public void GetHit(int damage)
    {
        if(_hitCooldown > 0)
        {
            _status.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Makes the boss vulnerable for the amount of time specified in the inspector.
    /// </summary>
    public void MakeVulnerable()
    {
        _hitCooldown = _cooldownTime;
    }

    /// <summary>
    /// Sets all of the doors to be closed except for the entrances which remain open.
    /// </summary>
    public void SetDoorsClosed()
    {
        _topDoor.ResetAndTurnOff();
        _bottomDoor.ResetAndTurnOff();
        foreach(Door door in _otherDoors)
        {
            if(typeof(RecordingDoor).IsInstanceOfType(door))
            {
                RecordingDoor rd = (RecordingDoor) door;
                rd.KeepOpen();
            }
            else
            {
                door.ResetAndTurnOff();
            }
        }
    }

    /// <summary>
    /// Instantiates the death prefab and destroys the main boss once the death animation ends.
    /// </summary>
    public void Death()
    {
        GameObject go = Instantiate(_deathPrefab, transform.position - Vector3.right, Quaternion.Euler(0, 0, 60));
        
        go.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 1000);

        transform.parent.GetChild(2).gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
