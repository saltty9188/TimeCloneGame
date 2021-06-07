using UnityEngine;

/// <summary>
/// The PlayerMovement class is responsible for handling all movement performed by the player and time-clones.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The movement speed of the player.")]
    [SerializeField] private float _moveSpeed = 5;
    [Tooltip("The initial force of the player's jump.")]
    [SerializeField] private float _jumpPower = 600;
    [Tooltip("The speed at which the player is knocked back.")]
    [SerializeField] private float _knockBackSpeed = 10;
    [Tooltip("The amount of time the knock back is effective for.")]
    [SerializeField] private float _knockBackDuration = 0.2f;
    [Tooltip("The distance from which the player can grab a box.")]
    [SerializeField] private float _boxGrabDistance = 0.75f;
    [Tooltip("Only check for boxes to grab.")]
    [SerializeField] private LayerMask _objectsOnly;
    #endregion

    #region Private fields
    private Aim _aimScript;
    private ToolTips _toolTips;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Vector2 _playerSize;
    private GameObject _nearbyLadder;
    private bool _onLadder;
    private bool _grounded;
    private bool _facingRight;
    private bool _nearBox;
    private Transform _originalParent;
    private float _knockBackTime;
    private Vector2 _knockBackDirection;
    private bool _conntectedToBox;
    #endregion

    void Start()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _playerSize = GetComponent<CapsuleCollider2D>().size;
        _originalParent = transform.parent;
        _aimScript = transform.GetChild(0).GetComponent<Aim>();
        _toolTips = GetComponent<ToolTips>();
    }

    void FixedUpdate()
    {
        bool wasGrounded = _grounded;

        //Disable jump animation upon landing
        if(_grounded)
        {
            _animator.SetBool("Jump", false); 
        } 
    }

    /// <summary>
    /// Moves the player in the given direction and causes them to jump and/or grab if needed.
    /// </summary>
    /// <param name="movement">The direction of movement for the player.</param>
    /// <param name="jumping">Whether or not the player is jumping.</param>
    /// <param name="grabbing">Whether or not the player is holding the grab button.</param>
    public void Move(Vector2 movement, bool jumping, bool grabbing)
    {
        if(this.enabled)
        {
            // Check if the player is near a box
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, _boxGrabDistance, _objectsOnly);
            _nearBox = hit && hit.collider.tag == "Box";

            // only display the arm sprite if it's needed
            if(_aimScript.CurrentWeapon == null && !_nearBox)
            {
                _aimScript.gameObject.SetActive(false);
                _aimScript.enabled = false;
            }
            else
            {
                _aimScript.gameObject.SetActive(true);
                _aimScript.GetComponent<SpriteRenderer>().enabled = true;
                _aimScript.enabled = true;
            }

            _animator.SetBool("HasWeapon", _aimScript.gameObject.activeSelf);

            // Set grab tool tip for the player only
            if(tag == "Player") _toolTips.GrabToolTip(_nearBox);
            _aimScript.GrabArm(_nearBox);

            // Attach the player to the box to allow pulling if the player is near a box and holding the grab button
            if(_nearBox)
            {
                FixedJoint2D joint = hit.collider.GetComponent<FixedJoint2D>();
                if(grabbing && _grounded && Mathf.Abs(hit.collider.GetComponent<Rigidbody2D>().velocity.y) <= 0.1f)
                {
                    joint.enabled = true;
                    joint.connectedBody = _rigidbody2D;
                    _aimScript.enabled = false;
                    _conntectedToBox = true;
                }
                else
                {
                    joint.enabled = false;
                    joint.connectedBody = null;
                    _aimScript.enabled = true;
                    _conntectedToBox = false;
                }
            }

            // Don't move the player if they are under the effect of knock back
            if(_knockBackTime > 0)
            {
                _knockBackTime -= Time.deltaTime;
                _rigidbody2D.velocity = new Vector2(_knockBackDirection.x * _knockBackSpeed, _rigidbody2D.velocity.y);
                _animator.SetFloat("Speed", 0);
            }
            else
            {
                // Start climbing a ladder if the player is nearby one and holding up or down
                if(_nearbyLadder && ((movement.y > 0.5 && transform.position.y < _nearbyLadder.transform.GetChild(0).position.y)
                    || movement.y < -0.5 && transform.position.y > _nearbyLadder.transform.position.y))
                {
                    _onLadder = true;
                    transform.position = new Vector3(_nearbyLadder.transform.position.x, transform.position.y, transform.position.z);
                }

                // Climb the ladder if on one
                if(_onLadder && _nearbyLadder)
                {
                    ClimbLadder(movement, jumping);
                }
                else
                {
                    // move the player as normal
                    OffLadder();
                    _rigidbody2D.velocity = new Vector2(movement.x * _moveSpeed, _rigidbody2D.velocity.y);

                    // Only flip the player based on movement if the player is not holding a weapon
                    if(!_aimScript.gameObject.activeSelf)
                    {
                        _facingRight = transform.localScale.x > 0;
                        if(movement.x < 0 && _facingRight)
                        {
                            Debug.Log(_facingRight);
                            Flip();
                        }
                        else if(movement.x > 0 && !_facingRight)
                        {
                            Flip();
                        }
                    }
                    else
                    {
                        // Use the running backwards animation if the player id running backwards with a weapon
                        bool runningBackwards = movement.x * transform.localScale.x < 0;
                        _animator.SetBool("RunningBackwards", runningBackwards);
                    }

                    //Jump if the player is grounded
                    if(jumping && _grounded && !_conntectedToBox)
                    {
                        Jump();
                    }

                    _animator.SetFloat("Speed", Mathf.Abs(movement.x));
                }
            }
        }
    }

    // Move up or down on the ladder
    void ClimbLadder(Vector2 movement, bool jumping)
    {
        SetArmActive(false);
        _animator.SetBool("OnLadder", _onLadder);
        _animator.SetBool("Jump", false);
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocity = new Vector2(0, 0);
        transform.Translate(0, movement.y * _moveSpeed * Time.deltaTime, 0, Space.World);

        // Pause the animation if not moving
        if(movement.y == 0)
        {
            _animator.enabled = false;
        }
        else
        {
            _animator.enabled = true;
        }

        // allow the player to jump off the ladder
        if(jumping)
        {
            _rigidbody2D.isKinematic = false;
            _animator.enabled = true;
            _onLadder = false;
            _animator.SetBool("OnLadder", _onLadder);
            _animator.SetFloat("Speed", 0);
            Jump();
        }
        else if(transform.position.y - _playerSize.y / 2 >= _nearbyLadder.transform.GetChild(0).position.y && movement.y >= 0)
        {
            // Get off from the top
            _rigidbody2D.isKinematic = false;
            _onLadder = false;
            _animator.SetBool("OnLadder", _onLadder);
        }
        else if((transform.position.y - _playerSize.y / 2) <= (_nearbyLadder.transform.GetChild(1).position.y) && movement.y <= 0)
        {
            // Get off from the bottom
            _rigidbody2D.isKinematic = false;
            _onLadder = false;
            _animator.SetBool("OnLadder", _onLadder);
        }
    }

    // Cause the player to jump
    void Jump()
    {
        _rigidbody2D.drag = 0;
        _rigidbody2D.AddForce(new Vector2(0, _jumpPower));
        _grounded = false;
        _animator.SetBool("Jump", true);
        AudioManager.Instance.PlaySFX("PlayerJump");
    }

    // Set whether or not the aim script and arm game object are active
    void SetArmActive(bool active)
    {
        transform.GetChild(0).GetComponent<Aim>().enabled = active;
        transform.GetChild(0).gameObject.SetActive(active);
    }

    // Parent the player to a lift or physics object to allow smooth movement when riding one
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

    // Check if the player is touching the ground
    void OnCollisionStay2D(Collision2D other)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);

        foreach(ContactPoint2D contact in contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if(angle < 40) {
                _grounded = true; 
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        transform.parent = _originalParent;
        _grounded = false;
    }  

    /// <summary>
    /// Receives knockback in the given direction.
    /// </summary>
    /// <param name="direction">The direction of the knock back.</param>
    public void ReceiveKnockBack(Vector2 direction)
    {
        _knockBackTime = _knockBackDuration;
        _knockBackDirection = direction;
    }

    /// <summary>
    /// Gets the player off of a ladder.
    /// </summary>
    public void OffLadder()
    {
        // Ensure the animator is enabled once off the ladder
        _animator.enabled = true;
        _rigidbody2D.isKinematic = false;
        _rigidbody2D.useFullKinematicContacts = false;
        _onLadder = false;
        _animator.SetBool("OnLadder", _onLadder);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Ladder")
        {
            _nearbyLadder = other.gameObject;
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Ladder")
        {    
            _nearbyLadder = null;
        }
    }

    void Flip()
    {
        Vector3 temp = gameObject.transform.localScale;
        temp.x *= -1;
        gameObject.transform.localScale = temp;
    }
}
