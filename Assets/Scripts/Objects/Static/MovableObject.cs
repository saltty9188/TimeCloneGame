using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{

    #region Private fields
    private Vector3 initialPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
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
