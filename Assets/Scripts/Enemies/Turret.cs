using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private float aimRadius = 10;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float fireCooldown = 0.3f;

    [SerializeField] private LayerMask everythingButBullet;
    #endregion
    
    #region Private fields
    private float accumulatedTime;

    private GameObject currentTarget;
    #endregion

    void Awake()
    {
        currentTarget = null;
    }

    void Update()
    {  
        if(currentTarget == null)
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

        if(currentTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentTarget.transform.position - transform.position, aimRadius, everythingButBullet);
            if(hit && (hit.collider.tag == "Player" || hit.collider.tag == "Clone"))
            {
                FaceTarget(currentTarget.transform.position);
                Shoot(currentTarget.transform.position - transform.position);
            }
            else
            {
                currentTarget = null;
            }
        }

        if(accumulatedTime < fireCooldown) accumulatedTime += Time.deltaTime;
    }

    void Shoot(Vector2 direction)
    {
        if(accumulatedTime >= fireCooldown)
        {
            GameObject go = Instantiate(projectilePrefab, transform.GetChild(0).GetChild(0).position, new Quaternion());
            Projectile p = go.GetComponent<Projectile>();
            p.direction = direction;
            p.SetShooter(gameObject);
            accumulatedTime = 0;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }
}
