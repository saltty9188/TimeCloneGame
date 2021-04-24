using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossScript : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private GameObject projectilePrefab;
    #endregion

    #region Public fields
    public Vector2 upperRight = new Vector2(97.5f, 8.25f);
    public Vector2 lowerLeft = new Vector2(72.5f, -0.5f);
    public bool onRight;
    public int shootingLandCount;
    public float verticalSpeed;
    #endregion

    #region Private fields
    private GameObject projectileParent;
    private Animator animator;
    private SecondBossStatus status;
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
        status = GetComponent<SecondBossStatus>();
        onRight = true;

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
        Vector2 direction = (onRight ? Vector2.left : Vector2.right);

        //aim at the player if the boss is facing them
        if(closestTarget != null && 
            ((onRight && closestTarget.transform.position.x <= transform.position.x) || (!onRight && closestTarget.transform.position.x >= transform.position.x)))
        {
            direction = closestTarget.transform.position - transform.position;  
        }
        else
        {
            //Add some variation in the y axis
            direction.y += Random.Range(-1.0f, 1.0f);
        }

        GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).position, new Quaternion());
        Projectile p = go.GetComponent<Projectile>();
        p.direction = direction;
        p.transform.localScale = new Vector3(2, 2, 2);
        p.SetShooter(gameObject, true);
        p.transform.parent = projectileParent.transform;
    }
    void OnCollisionEnter2D(Collision2D other)
    {    
        if(other.GetContact(0).normal == Vector2.up && animator.GetBool("Jump") && other.GetContact(0).collider.tag == "Reflective")
        {
            animator.SetBool("Jump", false);
            animator.SetTrigger("Land");
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
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
        onRight = true;
        shootingLandCount = 0;
        animator.SetBool("On Right", true);
        animator.SetBool("Jump", false);
        animator.SetInteger("Land Count", 0);

        animator.SetTrigger("Reset Fight");
    }
}
