using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private bool hasBase = false;
    [SerializeField] private float aimRadius = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private LayerMask everythingButBullet;
    #endregion
    
    #region Private fields
    private float accumulatedTime;
    private GameObject currentTarget;
    private bool collisionBelow = false;
    private bool collisionAbove = false;
    private Rigidbody2D aboveContact = null;
    private Rigidbody2D belowContact = null;
    #endregion

    void Start()
    {
        currentTarget = null;
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
            GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.direction = direction;
            p.SetShooter(gameObject);
            accumulatedTime = 0;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if(angle < 0.5f)
            {
                collisionBelow = true;
                belowContact = contact.collider.GetComponent<Rigidbody2D>();
            }
            else if(angle > 179.5f)
            {
                collisionAbove = true;
                aboveContact = contact.collider.GetComponent<Rigidbody2D>();
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aimRadius);
    }
}
