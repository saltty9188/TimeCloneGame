using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossScript : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private int damage = 5;
    

    [SerializeField] private float phaseTransitionTime = 10;

    #endregion

    #region Public fields
    public Vector3 initialPosition;
    #endregion

    #region Private fields
    private Animator animator;
    private GameObject closestTarget;
    private float shootTime;
    bool inFight;
    private Vector3 initialScale;
    #endregion

    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;

        animator = GetComponent<Animator>();
        shootTime = 0;
    }

    void Update()
    {
        SelectTarget();

        bool shooting = !animator.GetBool("Moving");

        if(shooting && inFight)
        {
            shootTime += Time.deltaTime;
            if(shootTime >= phaseTransitionTime)
            {
                animator.SetBool("Moving", true);
                shootTime = 0;
            }
        }

        if(!inFight && transform.position != initialPosition)
        {
            transform.position = initialPosition;
        }
    }

    public void Shoot()
    {
        if(closestTarget != null)
        {
            //Shoulder launcher 1
            Vector2 direction = closestTarget.transform.position - transform.position;
            GameObject go = Instantiate(bulletPrefab, transform.GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.noHitTime = 0.4f;
            p.damage = damage;
            p.direction = direction;
            p.transform.localScale = new Vector3(2, 2, 2);
            p.IgnoreCollision(transform.GetChild(2).gameObject);
            p.SetShooter(gameObject);

            //Shoulder launcher 2
            go = Instantiate(bulletPrefab, transform.GetChild(1).position, new Quaternion());
            p = go.GetComponent<Projectile>();
            p.noHitTime = 0.4f;
            p.damage = damage;
            p.direction = direction;
            p.transform.localScale = new Vector3(2, 2, 2);
            p.IgnoreCollision(transform.GetChild(2).gameObject);
            p.SetShooter(gameObject);
        }   
    }

    public void StartFight()
    {
        animator.SetTrigger("Start Fight");
        animator.ResetTrigger("Reset");
        inFight = true;
    }

    public void ResetBoss()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = initialPosition;
        transform.localScale = initialScale;
        shootTime = 0;
        inFight = false;

        GetComponent<FirstBossStatus>().ResetStatus();

        animator.ResetTrigger("Start Fight");
        animator.SetBool("OnRight", true);
        animator.SetBool("Moving", false);
        
        animator.SetTrigger("Reset");
        
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
}
