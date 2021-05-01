using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorBot : EnemyBehaviour
{

    #region Inspector fields
    [SerializeField] private float searchRadius = 10;
    [SerializeField] private float moveSpeed = 2;
    #endregion

    #region Private fields
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private Transform rotator;
    private GameObject target;
    private Vector3 leftBoundary;
    private Vector3 rightBoundary;

    bool justReset;
    #endregion

    protected override void Start()
    {
        base.Start();
        rotator = transform.GetChild(0);
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        FaceTarget(transform.position + Vector3.down);
        float padding = Vector3.Distance(transform.position, transform.GetChild(0).GetChild(0).position);
        leftBoundary = GetBoundary(-transform.right, padding);
        rightBoundary = GetBoundary(transform.right, padding);
    }

    // Update is called once per frame
    void Update()
    {
        SearchForTarget();
        if(target)
        {

            if(justReset)
            {
                justReset = false;
                return;
            }

            if(target.transform.position.x < transform.position.x && transform.position.x > leftBoundary.x)
            {
                rigidbody2D.velocity = Vector2.right * -moveSpeed;
            }
            else if(target.transform.position.x > transform.position.x && transform.position.x < rightBoundary.x)
            {
                rigidbody2D.velocity = Vector2.right * moveSpeed;
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;
            }

            FaceTarget(target.transform.position);
        }
        else
        {
            FaceTarget(transform.position + Vector3.down);
        }

        animator.SetBool("TargetNear", target != null);
    }

    void SearchForTarget()
    {
        GameObject closestTarget = null;
        float closestDist = float.MaxValue;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

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

        target = closestTarget;
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if(transform.position.x >= rightBoundary.x) newRotation = -60;
        if(transform.position.x <= leftBoundary.x) newRotation = -120;
        rotator.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    Vector3 GetBoundary(Vector3 direction, float padding = 0)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject != gameObject && hit.collider.gameObject != rotator.GetChild(0).gameObject)
            {
                Vector3 boundary = hit.point;
                boundary -= direction * padding;
                return boundary;
            }
        }

        return Vector3.zero;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        FaceTarget(transform.position + Vector3.down);
        target = null;
        rigidbody2D.velocity = Vector2.zero;
        justReset = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
