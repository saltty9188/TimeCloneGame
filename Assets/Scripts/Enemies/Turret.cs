using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBehaviour
{
    #region Inspector fields
    [SerializeField] public bool hasBase = false;
    [SerializeField] private float aimRadius = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int numExplosions = 2;
    [SerializeField] private LayerMask everythingButBullet;
    #endregion
    
    #region Private fields
    private float accumulatedTime;
    private GameObject currentTarget;
    public bool collisionBelow = false;
    public bool collisionAbove = false;
    private Rigidbody2D aboveContact = null;
    private Rigidbody2D belowContact = null;
    #endregion

    protected override void Start()
    {
        base.Start();
        EnemyManager.enemies.Add(gameObject);
    }

    void Update()
    {  
        if(currentTarget == null)
        {
            float closestDist = float.MaxValue;
            GameObject closestTarget = null;
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

                    if(dist < closestDist && dist <= aimRadius)
                    {
                        closestDist = dist;
                        closestTarget = target;
                    }
                }
            }
            currentTarget = closestTarget;
        }

        if(currentTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentTarget.transform.position - transform.position, aimRadius, everythingButBullet);
            if(hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
            {
                FaceTarget(currentTarget.transform.position);
                Shoot(currentTarget.transform.position - transform.position);
            }
            else
            {
                currentTarget = null;
            }
        }

        if(accumulatedTime < fireCooldown) accumulatedTime += Time.deltaTime;

        if(collisionAbove && collisionBelow)
        {
            if(hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if(accumulatedTime >= fireCooldown)
        {
            GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).GetChild(0).GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.direction = direction;
            p.damage = damage;
            p.SetShooter(gameObject);
            accumulatedTime = 0;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.collider.tag != "Player" && contact.collider.tag != "Clone" && contact.collider.tag != "Projectile")
            {
                
                if(contact.point.y < transform.position.y)
                {
                    collisionBelow = true;
                    belowContact = contact.collider.GetComponent<Rigidbody2D>();
                }
                else if(contact.point.y > transform.position.y)
                {
                    collisionAbove = true;
                    aboveContact = contact.collider.GetComponent<Rigidbody2D>();
                }
            }
        }

        if(collisionAbove && collisionBelow)
        {
            CreateExplosions();
            if(hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.point.y < transform.position.y)
            {
                collisionBelow = true;
                belowContact = contact.collider.GetComponent<Rigidbody2D>();
            }
            else if(contact.point.y > transform.position.y)
            {
                collisionAbove = true;
                aboveContact = contact.collider.GetComponent<Rigidbody2D>();
            }
        }

        if(collisionAbove && collisionBelow)
        {
            CreateExplosions();
            if(hasBase)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other) 
    {
        if(other.collider.GetComponent<Rigidbody2D>() == aboveContact)
        {
            aboveContact = null;
            collisionAbove = false;
        }
        else if(other.collider.GetComponent<Rigidbody2D>() == belowContact)
        {
            belowContact = null;
            collisionBelow = false;
        }
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        if(hasBase)
        {
            transform.parent.gameObject.SetActive(startActiveState);
        }
    }

    void CreateExplosions()
    {
        float explosionRadius = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4.0f;
        Vector3[] positions = new Vector3[numExplosions];
        for(int i = 0; i < numExplosions; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            position.Normalize();
            positions[i] = (position * explosionRadius * Random.Range(0.0f, 1.0f)) + transform.position;
        }

        EnemyManager.instance.SpawnExplosions(positions);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aimRadius);
    }
}
