using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private bool moving = false;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private Transform[] moveTransforms;
    #endregion

    #region Private fields
    private Rigidbody2D rigidbody2D;
    private Vector3 initialPosition;
    private Vector3[] movePoints;
    private Vector3 moveTo;
    private int moveToIndex;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        rigidbody2D = GetComponent<Rigidbody2D>();

        if(moving)
        {
            movePoints = new Vector3[moveTransforms.Length];
            for(int i = 0; i < movePoints.Length; i++)
            {
                movePoints[i] = moveTransforms[i].position;
            }
            
            
            moveToIndex = 0;
            moveTo = movePoints[moveToIndex];
        }
    }

    void Update()
    {
        if(moving)
        {
            if(Vector3.Distance(transform.position, moveTo) < 0.1f)
            {
                NextPoint();
            }
            else
            {
                Vector2 direction = moveTo - transform.position;
                direction.Normalize();
                rigidbody2D.velocity = direction * moveSpeed;
            }
        }
    }

    void NextPoint()
    {
        moveToIndex++;
        if(moveToIndex >= movePoints.Length)
        {
            moveToIndex = 0;
        }
        moveTo = movePoints[moveToIndex];
    }

    public void SetInitialPosition()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.transform.position = initialPosition;
    }
}
