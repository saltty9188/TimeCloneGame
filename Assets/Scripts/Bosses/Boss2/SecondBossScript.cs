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
    public const float UPPER_BOUND = 7.0f;
    public const float LOWER_BOUND = -2.0f;
    public const float RIGHT_BOUND = 97.5f;
    public const float LEFT_BOUND = 72.5f;
    public const float PLATFORM_HEIGHT = 2.0f;
    //public bool onRight;
    public int shootingLandCount;
    public float verticalSpeed;
    #endregion

    #region Private fields
    private GameObject projectileParent;
    private Animator animator;
    private BossStatus status;
    private GameObject closestTarget;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private string[] triggerNames;
    private float jumpCooldown;
    #endregion

    void Start()
    {
        projectileParent = new GameObject();
        animator = GetComponent<Animator>();
        status = GetComponent<BossStatus>();

        initialPosition = transform.position;
        initialScale = transform.localScale;

        triggerNames = new string[]
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
        animator.SetInteger("Land Count", shootingLandCount);
        SelectTarget();
        if(jumpCooldown > 0) jumpCooldown -= Time.deltaTime;
    }

    public void Flip()
    {
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    public void Shoot()
    {
        Vector2 direction = (animator.GetBool("On Right") ? Vector2.left : Vector2.right);

        //aim at the player if the boss is facing them
        if(closestTarget != null && 
            ((animator.GetBool("On Right") && closestTarget.transform.position.x <= transform.position.x) || 
            (!animator.GetBool("On Right") && closestTarget.transform.position.x >= transform.position.x)))
        {
            direction = closestTarget.transform.position - transform.position;  
        }
        else
        {
            //Add some variation in the y axis
            direction.y += Random.Range(-1.0f, 1.0f);
        }

        GameObject go = Instantiate(_projectilePrefab, transform.GetChild(0).position, new Quaternion());
        Projectile p = go.GetComponent<Projectile>();
        p.direction = direction;
        p.transform.localScale = new Vector3(2, 2, 2);
        p.SetShooter(gameObject, true);
        p.transform.parent = projectileParent.transform;

        AudioManager.Instance.PlaySFX("LaserFire", 0.7f);
    }
    
    void SelectTarget()
    {
        float closestDist = float.MaxValue;
        for(int i = 0; i < EnemyManager.targets.Count; i++)
        {
            if(EnemyManager.targets[i] == null)
            {
                EnemyManager.targets.RemoveAt(i);
                i--;
            }
            else
            {
                GameObject target = EnemyManager.targets[i];
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if(dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }     
        }
    }

    void ResetAllTriggers()
    {
        foreach(string trigger in triggerNames)
        {
            animator.ResetTrigger(trigger);
        }
    }

    void DestroyAllProjectiles()
    {
        Destroy(projectileParent);
        projectileParent = new GameObject();
    }

    public void ResetBoss()
    {
        transform.position = initialPosition;
        transform.localScale = initialScale;
        
        status.ResetStatus();
        animator.StopPlayback();

        ResetAllTriggers();
        DestroyAllProjectiles();
        shootingLandCount = 0;
        animator.SetBool("On Right", true);
        animator.SetBool("Jump", false);
        animator.SetInteger("Land Count", 0);

        animator.SetTrigger("Reset Fight");
    }

    public void Death()
    {
        Destroy(gameObject);
        Destroy(projectileParent);
    }
}
