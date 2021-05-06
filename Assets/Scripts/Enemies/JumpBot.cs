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
    private bool grounded;
    private bool knockedBack;
    #endregion


    // Update is called once per frame
    void Update()
    {
        if(knockBackTime > 0)
        {
            knockBackTime -= Time.deltaTime;
            rigidbody.velocity = new Vector2(knockBackDirection.x * knockBackSpeed, rigidbody.velocity.y);
            knockedBack = true;
        }
        else
        {
            GameObject target = SearchForTargets();
            if(target)
            {
                if(grounded)
                {
                    rigidbody.drag = 0;
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    rigidbody.AddForce(new Vector2(0, jumpPower));
                    grounded = false;
                    knockedBack = false;
                }
                else if(!knockedBack)
                {
                   rigidbody.velocity = new Vector2(moveSpeed * (transform.position.x - target.transform.position.x > 0 ? -1 : 1), rigidbody.velocity.y);
                }
            }
            else
            {
                rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
