using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBossScript : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float cooldownTime = 0.3f;
    [SerializeField] private float ventSwapDelay = 20;
    //TEMP
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private LayerMask ignoreSelf;
    [SerializeField] private Door topDoor;
    [SerializeField] private Door bottomDoor;
    [SerializeField] private Door[] otherDoors;
    #endregion

    #region Private fields
    private BossStatus status;
    private SpriteRenderer spriteRenderer;
    private GameObject closestTarget;
    private GameObject projectileParent;
    private float shootTime;
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
        InitialValues();

        projectileParent = new GameObject(name + " Projectiles");
        shootTime = shootDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(inFight)
        {
            SelectTarget();

            //shoot (temp)
            if(shootTime > 0)
            {
                shootTime -= Time.deltaTime;
            }
            else
            {
                Shoot();
                shootTime = shootDelay;
            }

            if(hitCooldown > 0)
            {
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
                    transform.GetChild(2).gameObject.SetActive(true);
                    transform.GetChild(3).gameObject.SetActive(false);
                    bottomDoor.AddActivation();
                    topDoor.RemoveActivation();
                }
                else
                {
                    transform.GetChild(3).gameObject.SetActive(true);
                    transform.GetChild(2).gameObject.SetActive(false);
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
        //TODO: shoot time reset
        shootTime = shootDelay;

        Destroy(projectileParent);
        projectileParent = new GameObject(name + " Projectiles");
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

                RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, float.MaxValue, ignoreSelf);

                if(dist < closestDist && hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }     
        }
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
        }
    }

    void InitialValues()
    {
        ventSwapCooldown = ventSwapDelay;
        topVulnerable = true;
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);
        bottomDoor.ResetEvent();
        bottomDoor.AddActivation();
        topDoor.ResetEvent();
        topDoor.RemoveActivation();

        inFight = false;
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
}
