using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBot : EnemyBehaviour
{

    #region Inspector fields
    [SerializeField] private float searchRadius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private int touchDamage = 20;
    #endregion

    #region Private fields
    private Animator animator;
    private GameObject target;
    private bool grounded;
    private bool canLand;
    #endregion

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(KnockBackTime > 0)
        {
            KnockBackTime -= Time.deltaTime;
            Rigidbody.velocity = new Vector2(KnockBackDirection.x * _knockBackSpeed, Rigidbody.velocity.y);
        }
        else
        {
            target = SearchForTargets();
            if(grounded && canLand)
            {
                animator.SetTrigger("Land");
            }

            if(target && !grounded)
            {
                Rigidbody.velocity = new Vector2(moveSpeed * (transform.position.x - target.transform.position.x > 0 ? -1 : 1), Rigidbody.velocity.y);
            }
            else
            {
                Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
            } 
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.GetContact(0).collider.tag == "Player" || other.GetContact(0).collider.tag == "Clone")
        {
            PlayerStatus ps = other.GetContact(0).collider.GetComponent<PlayerStatus>();
            if(ps)
            {
                ps.TakeDamage(touchDamage, -other.GetContact(0).normal);
            }
        }

        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if (angle < 40)
            {
                grounded = true;
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if (angle < 40)
            {
                grounded = true;
            }
        }

        if(other.GetContact(0).collider.tag == "Player" || other.GetContact(0).collider.tag == "Clone")
        {
            PlayerStatus ps = other.GetContact(0).collider.GetComponent<PlayerStatus>();
            if(ps)
            {
                ps.TakeDamage(touchDamage, -other.GetContact(0).normal);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        grounded = false;
    }

    GameObject SearchForTargets()
    {
        float closestDist = float.MaxValue;
        GameObject closestTarget = null;
        for (int i = 0; i < EnemyManager.targets.Count; i++)
        {
            if (EnemyManager.targets[i] == null)
            {
                EnemyManager.targets.RemoveAt(i);
                i--;
            }
            else
            {
                GameObject target = EnemyManager.targets[i];
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }
        }

        if(closestDist > searchRadius) closestTarget = null;

        return closestTarget;
    }

    public void LandingComplete()
    {
        animator.ResetTrigger("Land");
        animator.SetTrigger("Jump");
        canLand = false;
    }

    public void Jump()
    {
        animator.ResetTrigger("Jump");
        Rigidbody.drag = 0;
        Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
        Rigidbody.AddForce(new Vector2(0, jumpPower*2));
        grounded = false;
    }

    public void JumpFinished()
    {
        canLand = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
