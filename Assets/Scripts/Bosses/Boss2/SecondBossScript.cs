using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossScript class is the main class that drives the actions of the Second Boss.
/// </summary>
public class SecondBossScript : MonoBehaviour
{

    #region Inspector fields
    [Tooltip("Prefab for the bullets the boss fires.")]
    [SerializeField] private GameObject _projectilePrefab;
    #endregion

    #region Public fields
    /// <summary>
    /// The maxium height the boss will go when flying upwards.
    /// </summary>
    public const float UPPER_BOUND = 7.0f;

    /// <summary>
    /// The boss's Y position when it is touching the floor.
    /// </summary>
    public const float LOWER_BOUND = -2.0f;

    /// <summary>
    /// The maximum distance to the right that the boss will travel during the fight. 
    /// </summary>
    public const float RIGHT_BOUND = 97.5f;

    /// <summary>
    /// The maximum distance to the left that the boss will travel during the fight. 
    /// </summary>
    public const float LEFT_BOUND = 72.5f;

    /// <summary>
    /// The boss's Y position when it is touching one of the floating platforms.
    /// </summary>
    public const float PLATFORM_HEIGHT = 2.0f;

    /// <summary>
    /// Boolean value for determing if the current "Jump" animation is the first jump or the second one.
    /// </summary>
    [HideInInspector]
    public bool SecondJump;

    /// <summary>
    /// The vertical velocity of the boss that is used in various StateMachineBehaviour scripts.
    /// </summary>
    [HideInInspector]
    public float VerticalSpeed;
    #endregion

    #region Private fields
    private GameObject _projectileParent;
    private Animator _animator;
    private BossStatus _status;
    private GameObject _closestTarget;
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private string[] _triggerNames;
    #endregion

    void Start()
    {
        _projectileParent = new GameObject();
        _animator = GetComponent<Animator>();
        _status = GetComponent<BossStatus>();

        _initialPosition = transform.position;
        _initialScale = transform.localScale;

        _triggerNames = new string[]
        {
            "Start Fight",
            "Start Over-Run",
            "Start Over-Shoot",
            "Land",
            "Fly",
            "Drop"
        };

        //Ignore collisions with the player directly
        Physics2D.IgnoreLayerCollision(12, 8);
    }

    void Update()
    {
        SelectTarget();
    }

    /// <summary>
    /// Flips the boss sprite in the x axis.
    /// </summary>
    public void Flip()
    {
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    /// <summary>
    /// Shoots at the player if the boss is able to, otherwise shoots in a random direction in front of it.
    /// </summary>
    public void Shoot()
    {
        Vector2 direction = (_animator.GetBool("On Right") ? Vector2.left : Vector2.right);

        //aim at the player if the boss is facing them
        if(_closestTarget != null && 
            ((_animator.GetBool("On Right") && _closestTarget.transform.position.x <= transform.position.x) || 
            (!_animator.GetBool("On Right") && _closestTarget.transform.position.x >= transform.position.x)))
        {
            direction = _closestTarget.transform.position - transform.position;  
        }
        else
        {
            //Add some variation in the y axis
            direction.y += Random.Range(-1.0f, 1.0f);
        }

        GameObject go = Instantiate(_projectilePrefab, transform.GetChild(0).position, new Quaternion());
        Projectile p = go.GetComponent<Projectile>();
        p.Redirect(direction);
        p.transform.localScale = new Vector3(2, 2, 2);
        p.SetShooter(gameObject, true);
        p.transform.parent = _projectileParent.transform;

        AudioManager.Instance.PlaySFX("LaserFire", 0.7f);
    }
    
    void SelectTarget()
    {
        float closestDist = float.MaxValue;
        for(int i = 0; i < EnemyManager.Targets.Count; i++)
        {
            // Remove the target from the array if it's been destroyed
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

    void ResetAllTriggers()
    {
        foreach(string trigger in _triggerNames)
        {
            _animator.ResetTrigger(trigger);
        }
    }

    void DestroyAllProjectiles()
    {
        Destroy(_projectileParent);
        _projectileParent = new GameObject();
    }

    /// <summary>
    /// Resets the boss to how it was at the beginning of the fight.
    /// </summary>
    /// <remarks>
    /// Resets the the scale and position, and all of the animation parameters.
    /// </remarks>
    public void ResetBoss()
    {
        transform.position = _initialPosition;
        transform.localScale = _initialScale;
        
        _status.ResetStatus();
        _animator.StopPlayback();

        ResetAllTriggers();
        DestroyAllProjectiles();
        SecondJump = false;
        _animator.SetBool("On Right", true);
        _animator.SetBool("Jump", false);
        _animator.SetInteger("Land Count", 0);

        _animator.SetTrigger("Reset Fight");
    }

    /// <summary>
    /// Animation event that destroys the boss at the end of its death animation.
    /// </summary>
    public void Death()
    {
        Destroy(gameObject);
        Destroy(_projectileParent);
    }
}
