using UnityEngine;

/// <summary>
/// The CameraTracker class is used to make the main Camera follow the player.
/// </summary>
public class CameraTracker : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The player to track.")]
    [SerializeField] private Transform _playerTransform;
    [Tooltip("The vertical offset from the player's y position.")]
    [SerializeField] private float _verticalOffset = 4;
    [Tooltip("The min Y position the camera can go.")]
    [SerializeField] private float _minYPos;
    [Tooltip("The max Y position the camera can go.")]
    [SerializeField] private float _maxYPos;
    #endregion

    #region Public fields
    /// <value>The vertical offset from the player's Y position.</value>
    public float VerticalOffset
    {
        get {return _verticalOffset;}
    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        // clamp the camera y position
        float yPos = Mathf.Clamp(_playerTransform.position.y + _verticalOffset, _minYPos, _maxYPos);
        transform.position = new Vector3(_playerTransform.position.x, yPos, transform.position.z);
    }

    /// <summary>
    /// Updates the maximum and minimum Y position thresholds.
    /// </summary>
    /// <param name="min">The new minimum Y position.</param>
    /// <param name="max">The new maximum Y position.</param>
    public void UpdateMaxAndMin(float min, float max)
    {
        _minYPos = min;
        _maxYPos = max;
    }
}
