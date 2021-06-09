using UnityEngine;

/// <summary>
/// The JumpBot class controls the actions of JumpBot enemies.
/// </summary>
public class JumpBot : EnemyBehaviour
{
    #region Inspector fields
    [Tooltip("The radius that the enemy will search for targets in.")]
    [SerializeField] private float _searchRadius = 10;
    [Tooltip("How fast the enemy moves.")]
    [SerializeField] private float _moveSpeed = 3;
    [Tooltip("How high the enemy jumps.")]
    [SerializeField] private float _jumpPower = 400;
    [Tooltip("The amount of damage the enemy does.")]
    [SerializeField] private int _touchDamage = 20;
    #endregion

    #region Private fields
    private Animator _animator;
    private GameObject _target;
    private bool _grounded;
    private bool _canLand;
    #endregion

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        if(KnockBackTime > 0)
        {
            KnockBackTime -= Time.deltaTime;
            Rigidbody.velocity = new Vector2(KnockBackDirection.x * KnockBackSpeed, Rigidbody.velocity.y);
        }
        else
        {
            _target = SearchForTargets();
            if(_grounded && _canLand)
            {
                _animator.SetTrigger("Land");
            }

            if(_target && !_grounded)
            {
                Rigidbody.velocity = new Vector2(_moveSpeed * (transform.position.x - _target.transform.position.x > 0 ? -1 : 1), Rigidbody.velocity.y);
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
                ps.TakeDamage(_touchDamage, -other.GetContact(0).normal);
            }
        }

        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if (angle < 40)
            {
                _grounded = true;
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
                _grounded = true;
            }
        }

        if(other.GetContact(0).collider.tag == "Player" || other.GetContact(0).collider.tag == "Clone")
        {
            PlayerStatus ps = other.GetContact(0).collider.GetComponent<PlayerStatus>();
            if(ps)
            {
                ps.TakeDamage(_touchDamage, -other.GetContact(0).normal);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        _grounded = false;
    }

    GameObject SearchForTargets()
    {
        float closestDist = float.MaxValue;
        GameObject closestTarget = null;
        for (int i = 0; i < EnemyManager.Targets.Count; i++)
        {
            if (EnemyManager.Targets[i] == null)
            {
                EnemyManager.Targets.RemoveAt(i);
                i--;
            }
            else
            {
                GameObject target = EnemyManager.Targets[i];
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = target;
                }
            }
        }

        if(closestDist > _searchRadius) closestTarget = null;

        return closestTarget;
    }

    /// <summary>
    /// Animation event that determines the landing animation is complete.
    /// </summary>
    public void LandingComplete()
    {
        _animator.ResetTrigger("Land");
        _animator.SetTrigger("Jump");
        _canLand = false;
    }

    /// <summary>
    /// Animation event that causes the enemy to jump.
    /// </summary>
    public void Jump()
    {
        _animator.ResetTrigger("Jump");
        Rigidbody.drag = 0;
        Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
        Rigidbody.AddForce(new Vector2(0, _jumpPower));
        _grounded = false;
    }

    /// <summary>
    /// Animation event that indicates the jump animation is complete.
    /// </summary>
    public void JumpFinished()
    {
        _canLand = true;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}
