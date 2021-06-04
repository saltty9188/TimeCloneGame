using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SpiderBot class controls the actions of SpiderBot enemies.
/// </summary>
public class SpiderBot : EnemyBehaviour
{
    #region Inspector fields
    [SerializeField] private float _searchRadius = 10;
    [SerializeField] private float _movementSpeed = 2;
    [SerializeField] private LayerMask _runAwayMask;
    #endregion

    #region Private fields
    private Animator _animator;
    private GameObject _currentTarget;
    private GameObject _heldObject;
    private Transform _heldObjectParent;
    private Vector2 _moveDirection;
    #endregion

    
    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Just move when holding an object
        if(_heldObject)
        {
            Rigidbody.velocity = _moveDirection * _movementSpeed;
        }
        else
        {
            SearchForObject();
            // Move towards the current target if there is one
            if(_currentTarget)
            {
                _moveDirection = _currentTarget.transform.position - transform.position;
                _moveDirection.Normalize();
                Rigidbody.velocity = _moveDirection * _movementSpeed;
            }
            else
            {
                _moveDirection = Vector2.zero;
                Rigidbody.velocity = Vector2.zero;
            }
        }
    }

    void SearchForObject()
    {
        GameObject closestTarget = null;
        float closestDist = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _searchRadius);

        foreach(Collider2D collider in colliders)
        {
            if(collider.GetComponent<MovableObject>())
            {
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                // Only count the object if its not already held by another spiderbot
                if(dist < closestDist && collider.transform.parent.tag != "Enemy")
                {
                    closestDist = dist;
                    closestTarget = collider.gameObject;
                }
            }
        }

        _currentTarget = closestTarget;
        _animator.SetBool("HasTarget", _currentTarget != null);
    }

    // Returns true if the potentialChild is not a child of the parent
    bool NotChild(GameObject potentialChild, GameObject parent)
    {
        if(parent == null)
        {
            return true;
        }
        if(parent == potentialChild)
        {
            return false;
        }
        else
        {
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if(potentialChild == child)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Drops the MovableObject held by the SpiderBot if there is one.
    /// </summary>
    public void DropObject()
    {
        if(_heldObject)
        {
            IgnoreCollisionWithHeld(false);
            _heldObject.transform.parent = _heldObjectParent;
            _heldObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            _heldObject = null;
            _animator.SetBool("HoldingObject", false);
        }
    }

    // Ignores collision with the held object and its children
    void IgnoreCollisionWithHeld(bool ignore)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), _heldObject.GetComponent<Collider2D>(), ignore);
        for(int i = 0; i < _heldObject.transform.childCount; i++)
        {
            Collider2D child = _heldObject.transform.GetChild(i).GetComponent<Collider2D>();
            if(child)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), child, ignore);
            }
        }
    }

    /// <summary>
    /// Resets the enemy to its cached position, scale, speed, and active state.
    /// </summary>
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        DropObject();
        _currentTarget = null;
        _animator.ResetTrigger("Reset");
        Rigidbody.velocity = Vector2.zero;
        _moveDirection = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.GetContact(0).collider.gameObject != _currentTarget && _heldObject)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
            other.GetContacts(contacts);

            foreach(ContactPoint2D contact in contacts)
            {
                // if the contact's collision layer is in the layer mask
                // https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
                // Have the spiderbot bounce off it if the contact normal is in the oposite direction to the movement direction
                if((_runAwayMask == (_runAwayMask | (1 << contact.collider.gameObject.layer))) 
                    && contact.collider.gameObject != gameObject && NotChild(contact.collider.gameObject, _heldObject) 
                    && Vector2.Dot(_moveDirection, contact.normal) < 0)
                {
                    _moveDirection = Vector2.Reflect(_moveDirection, contact.normal);
                    return;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if((other.GetContact(0).collider.gameObject == _currentTarget || !NotChild(other.GetContact(0).collider.gameObject, _currentTarget)) && !_heldObject)
        {
            _heldObjectParent = _currentTarget.transform.parent;
            _heldObject = _currentTarget;
            _heldObject.transform.parent = transform;
            _heldObject.transform.position = transform.position - new Vector3(0, 0.4f, 0);
            IgnoreCollisionWithHeld(true);
            

            _animator.SetBool("HasTarget", false);
            _animator.SetBool("HoldingObject", true);
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
    
}
