using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ThirdBossScript : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject deathPrefab;
    [SerializeField] private float cooldownTime = 0.3f;
    [SerializeField] private float ventSwapDelay = 20;
    [SerializeField] private LayerMask ignoreSelf;
    [SerializeField] private Door topDoor;
    [SerializeField] private Door bottomDoor;
    [SerializeField] private Door[] otherDoors;
    #endregion

    #region Private fields
    private BossStatus status;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Animator frontVent;
    private Animator topVent;
    private Light2D shootLight;
    private GameObject closestTarget;
    private GameObject projectileParent;
    private bool inFight;
    private float hitCooldown;
    private bool topVulnerable;
    private float ventSwapCooldown;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<BossStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        shootLight = GetComponentInChildren<Light2D>();   

        topVent =  transform.GetChild(2).GetComponent<Animator>();
        frontVent = transform.parent.GetChild(1).GetComponent<Animator>();

        InitialValues();

        projectileParent = new GameObject(name + " Projectiles");
    }

    // Update is called once per frame
    void Update()
    {
        if(inFight)
        {
            SelectTarget();

            if(hitCooldown > 0)
            {
                // remove or change later?
                spriteRenderer.color = Color.cyan;
                hitCooldown -= Time.deltaTime;
            } 
            else
            {
                spriteRenderer.color = Color.white;
            }

            if(ventSwapCooldown > 0)
            {
                ventSwapCooldown -= Time.deltaTime;
            }
            else
            {
                ventSwapCooldown = ventSwapDelay;
                topVulnerable = !topVulnerable;
                if(topVulnerable)
                {
                    topVent.SetBool("IsOpen", true);
                    frontVent.SetBool("IsOpen", false);
                    bottomDoor.AddActivation();
                    topDoor.RemoveActivation();
                }
                else
                {
                    topVent.SetBool("IsOpen", false);
                    frontVent.SetBool("IsOpen", true);
                    topDoor.AddActivation();
                    bottomDoor.RemoveActivation();
                }
            }
        }
    }

    public void StartFight()
    {
        inFight = true;
        Debug.Log("started");

    }

    public void ResetBoss()
    {
        status.ResetStatus();
        hitCooldown = 0;
        InitialValues();

        Destroy(projectileParent);
        projectileParent = new GameObject(name + " Projectiles");
    }

    void SelectTarget()
    {
        float closestDist = float.MaxValue;
        closestTarget = null;
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

                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, float.MaxValue, ignoreSelf);

                if(dist < closestDist && hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }     
        }

        animator.SetBool("targetFound", closestTarget);
    }

    public void Shoot()
    { 
        if(closestTarget != null)
        {
            Vector2 direction = closestTarget.transform.position - transform.position;

            GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.direction = direction;
            p.transform.localScale = new Vector3(2, 2, 2);
            p.SetShooter(gameObject, true);
            p.transform.parent = projectileParent.transform;

            AudioManager.instance.PlaySFX("LaserFire", 0.7f);
        }
    }

    void InitialValues()
    {
        ventSwapCooldown = ventSwapDelay;
        topVulnerable = true;
        animator.SetBool("targetFound", false);
        topVent.SetBool("IsOpen", true);
        frontVent.SetBool("IsOpen", false);
        bottomDoor.ResetEvent();
        bottomDoor.AddActivation();
        topDoor.ResetEvent();
        topDoor.RemoveActivation();

        inFight = false;
        closestTarget = null;
    }

    public void GetHit(int damage)
    {
        if(hitCooldown > 0)
        {
            status.TakeDamage(damage);
        }
    }

    public void MakeVulnerable()
    {
        hitCooldown = cooldownTime;
    }

    public void SetDoorsClosed()
    {
        topDoor.ResetAndTurnOff();
        bottomDoor.ResetAndTurnOff();
        foreach(Door door in otherDoors)
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

    public void Death()
    {
        GameObject go = Instantiate(deathPrefab, transform.position - Vector3.right, Quaternion.Euler(0, 0, 60));
        
        go.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 500);

        transform.parent.GetChild(2).gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
