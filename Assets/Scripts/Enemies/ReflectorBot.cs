using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ReflectorBot class controls the actions of ReflectorBot enemies.
/// </summary>
public class ReflectorBot : EnemyBehaviour
{
    #region Inspector fields
    [Tooltip("The radius that the enemy will search for targets in.")]
    [SerializeField] private float _searchRadius = 10;
    [Tooltip("How fast the enemy moves.")]
    [SerializeField] private float _moveSpeed = 2;
    #endregion

    #region Private fields
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Transform _rotator;
    private GameObject _target;
    private float _leftBoundary;
    private float _rightBoundary;

    bool justReset;
    #endregion

    protected override void Start()
    {
        base.Start();
        _rotator = transform.GetChild(0);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        FaceTarget(transform.position + Vector3.down);
        float padding = Vector3.Distance(transform.position, transform.GetChild(0).GetChild(0).position);
        _leftBoundary = GetBoundary(-transform.right, padding);
        _rightBoundary = GetBoundary(transform.right, padding);
    }

    // Update is called once per frame
    void Update()
    {
        SearchForTarget();
        if(_target)
        {
            // One frame buffer after reset
            if(justReset)
            {
                justReset = false;
                return;
            }

            // Move to block the target
            if(_target.transform.position.x < transform.position.x && transform.position.x > _leftBoundary)
            {
                _rigidbody2D.velocity = Vector2.right * -_moveSpeed;
            }
            else if(_target.transform.position.x > transform.position.x && transform.position.x < _rightBoundary)
            {
                _rigidbody2D.velocity = Vector2.right * _moveSpeed;
            }
            else
            {
                _rigidbody2D.velocity = Vector2.zero;
            }

            FaceTarget(_target.transform.position);
        }
        else
        {
            // Face the mirror down if there is no target
            FaceTarget(transform.position + Vector3.down);
        }

        _animator.SetBool("TargetNear", _target != null);
    }

    void SearchForTarget()
    {
        GameObject closestTarget = null;
        float closestDist = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _searchRadius);

        foreach(Collider2D collider in colliders)
        {
            if(collider.tag == "Player" || collider.tag == "Clone")
            {
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = collider.gameObject;
                }
            }
        }

        _target = closestTarget;
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Don't let the mirror rotate too far
        if(transform.position.x >= _rightBoundary) newRotation = -60;
        if(transform.position.x <= _leftBoundary) newRotation = -120;
        _rotator.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    float GetBoundary(Vector3 direction, float padding = 0)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject != gameObject && hit.collider.gameObject != _rotator.GetChild(0).gameObject)
            {
                Vector3 boundary = hit.point;
                boundary -= direction * padding;
                return boundary.x;
            }
        }
        return 0;
    }

    /// <summary>
    /// Resets the enemy to its cached position, scale, speed, and active state with the mirror facing down.
    /// </summary>
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        FaceTarget(transform.position + Vector3.down);
        _target = null;
        _rigidbody2D.velocity = Vector2.zero;
        justReset = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}
