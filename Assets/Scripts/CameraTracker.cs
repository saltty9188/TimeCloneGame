using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    
    #region Inspector fields
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float verticalOffset = 4;
    [SerializeField] private float maxYPos;
    [SerializeField] private float minYPos;
    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float yPos = Mathf.Clamp(playerTransform.position.y + verticalOffset, minYPos, maxYPos);
        transform.position = new Vector3(playerTransform.position.x, yPos, transform.position.z);
    }

    public void UpdateMaxAndMin(float min, float max)
    {
        minYPos = min;
        maxYPos = max;
    }
}
