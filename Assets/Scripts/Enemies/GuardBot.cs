using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBot : EnemyBehaviour
{

    #region Inspector fields
    [SerializeField] private float aimRadius = 10;
    [SerializeField] private float moveBoundaryWidth = 10;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private int damage = 5;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private LayerMask everythingButBullet;
    #endregion
    
    #region Private fields
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private float leftBoundary;
    private float rightBoundary;
    private GameObject currentTarget;
    private bool collisionBelow = false;
    private bool collisionAbove = false;
    private Rigidbody2D aboveContact = null;
    private Rigidbody2D belowContact = null;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        leftBoundary = transform.position.x - moveBoundaryWidth / 2f;
        rightBoundary = transform.position.x + moveBoundaryWidth / 2f;
        currentTarget = null;
        EnemyManager.enemies.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTarget == null)
        {
            Patrol();
            AcquireTarget();
        }

        if(currentTarget != null)
        {
            bool hitTarget = false;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, currentTarget.transform.position - transform.position, aimRadius, everythingButBullet);
            if(hits != null)
            {
                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.tag == "Player" || hit.collider.tag == "Clone")
                    {
                        hitTarget = true;
                    }
                }
            }

            if(hitTarget)
            {
                //shooting animation
                animator.SetBool("EnemyNearby", true);
                MoveTowardTarget();
            }
            else
            {
                animator.SetBool("EnemyNearby", false);
                currentTarget = null;
            }
        }

        //Get crushed
        if(collisionAbove && collisionBelow)
        {
            gameObject.SetActive(false);
        }
    }

    void AcquireTarget()
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

    void Patrol()
    {
        rigidbody2D.velocity = new Vector2(moveSpeed, 0);
        if((transform.position.x >= rightBoundary || AtEdge(true)) && moveSpeed > 0)
        {
            moveSpeed *= -1;
            Flip();
        }

        if((transform.position.x <= leftBoundary || AtEdge(false)) && moveSpeed < 0)
        {
            moveSpeed *= -1;
            Flip();
        }
    }

    void MoveTowardTarget()
    {
        Vector2 spriteSize = GetComponent<SpriteRenderer>().bounds.size;

        if(currentTarget.transform.position.x <= transform.position.x - spriteSize.x / 2f)
        {
            if(moveSpeed > 0)
            {
                moveSpeed *= -1;
                Flip();
            }
        }
        else if(currentTarget.transform.position.x >= transform.position.x + spriteSize.x / 2f)
        {
            if(moveSpeed < 0)
            {
                moveSpeed *= -1;
                Flip();
            }
        }

        if(AtEdge(moveSpeed > 0))
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            rigidbody2D.velocity = new Vector2(moveSpeed, 0);
        }
    }

    bool AtEdge(bool rightSide)
    {
        Vector2 spriteSize = GetComponent<SpriteRenderer>().bounds.size;

        Vector2 raycastPoint = new Vector2();
        if(rightSide)
        {
            raycastPoint = new Vector2(transform.position.x + spriteSize.x / 2f, transform.position.y - spriteSize.y / 2f);
        }
        else
        {
            raycastPoint = new Vector2(transform.position.x - spriteSize.x / 2f, transform.position.y - spriteSize.y / 2f);
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastPoint, Vector2.down, 0.1f);
        if(hits != null)
        {
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.tag != tag)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Shoot()
    {
        Vector2 direction = currentTarget.transform.position - transform.position;
        GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).position, new Quaternion());
        Projectile p = go.GetComponent<Projectile>();
        p.direction = direction;
        p.damage = damage;
        p.SetShooter(gameObject);     
    }

    void Flip()
    {
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        moveSpeed = Mathf.Abs(moveSpeed);
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
