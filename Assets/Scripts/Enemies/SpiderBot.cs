using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBot : EnemyBehaviour
{

    #region Inspector fields
    [SerializeField] private float searchRadius = 10;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private LayerMask runAwayMask;
    #endregion

    #region Private fields
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private GameObject currentTarget;
    private GameObject heldObject;
    private Transform heldObjectParent;
    private Vector2 moveDirection;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(heldObject)
        {
            rigidbody2D.velocity = moveDirection * movementSpeed;
        }
        else
        {
            SearchForObject();
            if(currentTarget)
            {
                moveDirection = currentTarget.transform.position - transform.position;
                moveDirection.Normalize();
                rigidbody2D.velocity = moveDirection * movementSpeed;
            }
            else
            {
                moveDirection = Vector2.zero;
                rigidbody2D.velocity = Vector2.zero;
            }
        }
    }

    void SearchForObject()
    {
        GameObject closestTarget = null;
        float closestDist = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach(Collider2D collider in colliders)
        {
            if(collider.GetComponent<MovableObject>())
            {
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = collider.gameObject;
                }
            }
        }

        currentTarget = closestTarget;
        animator.SetBool("HasTarget", currentTarget != null);
    }

    bool NotChild(GameObject go, GameObject parent)
    {
        if(parent == null)
        {
            return true;
        }
        if(parent == go)
        {
            return false;
        }
        else
        {
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if(go == child)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void DropObject()
    {
        if(heldObject)
        {
            IgnoreCollisionWithHeld(false);
            heldObject.transform.parent = heldObjectParent;
            heldObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            heldObject = null;
            animator.SetBool("HoldingObject", false);
        }
    }

    void IgnoreCollisionWithHeld(bool ignore)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), heldObject.GetComponent<Collider2D>(), ignore);
        for(int i = 0; i < heldObject.transform.childCount; i++)
        {
            Collider2D child = heldObject.transform.GetChild(i).GetComponent<Collider2D>();
            if(child)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), child, ignore);
            }
        }
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        DropObject();
        currentTarget = null;
        animator.ResetTrigger("Reset");
        rigidbody2D.velocity = Vector2.zero;
        moveDirection = Vector2.zero;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.GetContact(0).collider.gameObject != currentTarget && heldObject)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
            other.GetContacts(contacts);

            foreach(ContactPoint2D contact in contacts)
            {
                if((runAwayMask == (runAwayMask | (1 << contact.collider.gameObject.layer))) 
                    && contact.collider.gameObject != gameObject && NotChild(contact.collider.gameObject, heldObject))
                {
                    moveDirection = Vector2.Reflect(moveDirection, contact.normal);
                    return;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if((other.GetContact(0).collider.gameObject == currentTarget || !NotChild(other.GetContact(0).collider.gameObject, currentTarget)) && !heldObject)
        {
            heldObjectParent = currentTarget.transform.parent;
            heldObject = currentTarget;
            heldObject.transform.parent = transform;
            heldObject.transform.position = transform.position - new Vector3(0, 0.4f, 0);
            IgnoreCollisionWithHeld(true);
            

            animator.SetBool("HasTarget", false);
            animator.SetBool("HoldingObject", true);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
