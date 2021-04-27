using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float knockBackSpeed = 6;
    [SerializeField] private float knockBackDuration = 0.25f;
    [SerializeField] private float boxGrabDistance = 0.75f;
    [SerializeField] private LayerMask allButPlayer;
    #endregion

    #region Private fields
    private Aim aimScript;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Vector2 playerSize;
    private GameObject nearbyLadder;
    private bool betweenTwoLadders;
    private bool onLadder;
    private bool grounded;
    private bool grabbing;
    private Transform originalParent;
    protected float knockBackTime;
    protected Vector2 knockBackDirection;
    #endregion

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        playerSize = GetComponent<CapsuleCollider2D>().size;
        originalParent = transform.parent;
        aimScript = transform.GetChild(0).GetComponent<Aim>();
    }

    void FixedUpdate()
    {
        bool wasGrounded = grounded;

        //Disable jump animation upon landing
        if(grounded && wasGrounded) 
        {
            animator.SetBool("Jump", false); 
        } 
    }

    public void move(Vector2 movement, bool jumping, bool grabbing)
    {
        this.grabbing = grabbing;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, boxGrabDistance, allButPlayer);
        if(hit && hit.collider.tag == "Box")
        {
            FixedJoint2D joint = hit.collider.GetComponent<FixedJoint2D>();
            if(grabbing)
            {
                joint.enabled = true;
                joint.connectedBody = rigidbody;
                aimScript.enabled = false;
            }
            else
            {
                joint.enabled = false;
                joint.connectedBody = null;
                aimScript.enabled = true;
            }
        }

        if(knockBackTime > 0)
        {
            knockBackTime -= Time.deltaTime;
            rigidbody.velocity = new Vector2(knockBackDirection.x * knockBackSpeed, rigidbody.velocity.y);
            animator.SetFloat("Speed", 0);
        }
        else
        {

            if(nearbyLadder && ((movement.y > 0 && transform.position.y < nearbyLadder.transform.GetChild(0).position.y)
                || movement.y < 0 && transform.position.y > nearbyLadder.transform.position.y))
            {
                onLadder = true;
                transform.position = new Vector3(nearbyLadder.transform.position.x, transform.position.y, transform.position.z);
            }

            if(onLadder)
            {
                ClimbLadder(movement, jumping);
            }
            else
            {
                rigidbody.velocity = new Vector2(movement.x * moveSpeed, rigidbody.velocity.y);

                //Put in if statement so it doesn't get reset until the player hits the ground
                if(jumping && grounded)
                {
                    //rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
                    Jump();
                }

                animator.SetFloat("Speed", Mathf.Abs(movement.x));
            }
        }
    }

    void ClimbLadder(Vector2 movement, bool jumping)
    {
        SetArmActive(false);
        animator.SetBool("OnLadder", onLadder);
        animator.SetBool("Jump", false);
        rigidbody.isKinematic = true;
        rigidbody.velocity = new Vector2(0, 0);
        transform.Translate(0, movement.y * moveSpeed * Time.deltaTime, 0, Space.World);

        if(movement.y == 0)
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
        }

        if(jumping)
        {
            rigidbody.isKinematic = false;
            animator.enabled = true;
            onLadder = false;
            animator.SetBool("OnLadder", onLadder);
            animator.SetFloat("Speed", 0);
            Jump();
            SetArmActive(true);
        }
        else if(transform.position.y - playerSize.y / 2 >= nearbyLadder.transform.GetChild(0).position.y && movement.y >= 0)
        {
            //transform.position = new Vector3(transform.position.x, nearbyLadder.transform.GetChild(0).position.y + playerSize.y / 2, transform.position.z);
            rigidbody.isKinematic = false;
            onLadder = false;
            animator.SetBool("OnLadder", onLadder);
            SetArmActive(true);
        }
        else if((transform.position.y - playerSize.y / 2) <= (nearbyLadder.transform.GetChild(1).position.y) && movement.y <= 0)
        {
            rigidbody.isKinematic = false;
            onLadder = false;
            animator.SetBool("OnLadder", onLadder);
            SetArmActive(true);
        }
    }

    void Jump()
    {
        rigidbody.drag = 0;
        rigidbody.AddForce(new Vector2(0, jumpPower));
        grounded = false;
        animator.SetBool("Jump", true);
    }

    void SetArmActive(bool active)
    {
        transform.GetChild(0).GetComponent<Aim>().enabled = active;
        transform.GetChild(0).gameObject.SetActive(active);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Lift l = other.GetContact(0).collider.GetComponent<Lift>();
        if(l != null)
        {
            transform.parent = other.GetContact(0).collider.transform;
        }

        float angle = Vector2.Angle(other.GetContact(0).normal, Vector2.up);

        PhysicsObject po = other.GetContact(0).collider.GetComponent<PhysicsObject>();
        if(po && angle < 45)
        {
            transform.parent = other.GetContact(0).collider.transform;
        }

    }

    void OnCollisionStay2D(Collision2D other)
    {
        float angle = Vector2.Angle(other.GetContact(0).normal, Vector2.up);

        if(angle < 40) {
            grounded = true; 
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        transform.parent = originalParent;
        grounded = false;

        
    }  

    public void ReceiveKnockBack(Vector2 direction)
    {
        knockBackTime = knockBackDuration;
        knockBackDirection = direction;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Ladder")
        {
            if(nearbyLadder)
            {
                betweenTwoLadders = true;
            }
            nearbyLadder = other.gameObject;
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Ladder")
        {
            if(betweenTwoLadders)
            {
                betweenTwoLadders = false;
            }
            else
            {
                nearbyLadder = null;
            }
        }
    }
}
