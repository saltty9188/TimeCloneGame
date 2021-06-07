using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MirrorMover class is responsible for managing objects that can be moved by the MirrorMover.
/// </summary>
public class MovableObject : MonoBehaviour
{
    #region Private fields
    private Vector3 _initialPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
    }

    /// <summary>
    /// Updates the initial position of this MovableObject.
    /// </summary>
    public void SetInitialPosition()
    {
        _initialPosition = transform.position;
    }

    /// <summary>
    /// Resets the position of this MovableObject.
    /// </summary>
    public void ResetPosition()
    {
        transform.transform.position = _initialPosition;
    }
}
