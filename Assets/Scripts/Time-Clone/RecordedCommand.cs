using UnityEngine;

/// <summary>
/// The RecordedCommand struct holds information about the player's recorded movements in a given frame.
/// </summary>
public struct RecordedCommand
{
    #region Public fields
    /// <summary>
    /// The movement vector for this command.
    /// </summary>
    public Vector2 Movement;
    /// <summary>
    /// Whether or not the player was holding the jump button for this command.
    /// </summary>
    public bool Jumping;
    /// <summary>
    /// The aim angle for the arm for this command.
    /// </summary>
    public float AimAngle;
    /// <summary>
    /// Whether or not the player was holding the shoot button for this command.
    /// </summary>
    public bool Shooting;
    /// <summary>
    /// The accumulated time since the recording started.
    /// </summary>
    public float AccumulatedTime;
    /// <summary>
    /// The value for switching the current ray type.
    /// </summary>
    /// <remarks>
    /// Positive if next, negative if previous, and 0 if none.
    /// </remarks>
    public float RaySwitchValue;
    /// <summary>
    /// Whether or not the player was holding the grab button for this command.
    /// </summary>
    public bool Grabbing;
    /// <summary>
    /// Whether or not the player was using a MirrorMover during this command.
    /// </summary>
    public bool MovingMirror;
    /// <summary>
    /// The value for switching the current object on the MirrorMover.
    /// </summary>
    /// <remarks>
    /// Positive if next, negative if previous, and 0 if none.
    /// </remarks>
    public float MirrorMoveValue;
    /// <summary>
    /// The new weapon picked up by the player on this command. Null if no new weapon was picked up.
    /// </summary>
    public GameObject NewWeapon;
    #endregion
    
    /// <summary>
    /// Creates a new RecordedCommand.
    /// </summary>
    /// <param name="movement">The movement vector for this command.</param>
    /// <param name="jumping">Whether or not the player was holding the jump button for this command.</param>
    /// <param name="aimAngle">The aim angle for the arm for this command.</param>
    /// <param name="shooting">Whether or not the player was holding the shoot button for this command.</param>
    /// <param name="time">The accumulated time since the recording started.</param>
    /// <param name="raySwitchValue">The value for switching the current ray type. Positive if next, negative if previous, and 0 if none.</param>
    /// <param name="grabbing">Whether or not the player was holding the grab button for this command.</param>
    /// <param name="movingMirror">Whether or not the player was using a MirrorMover during this command.</param>
    /// <param name="mirrorMoveValue">The value for switching the current object on the MirrorMover. Positive if next, negative if previous, and 0 if none.</param>
    /// <param name="newWeapon">The new weapon picked up by the player on this command. Null if no new weapon was picked up.</param>
    public RecordedCommand(Vector2 movement, bool jumping, float aimAngle, bool shooting, float time, float raySwitchValue, bool grabbing, bool movingMirror, float mirrorMoveValue, GameObject newWeapon = null)
    {
        this.Movement = movement;
        this.Jumping = jumping;
        this.AimAngle = aimAngle;
        this.Shooting = shooting;
        this.AccumulatedTime = time;
        this.RaySwitchValue = raySwitchValue;
        this.Grabbing = grabbing;
        this.MovingMirror = movingMirror;
        this.MirrorMoveValue = mirrorMoveValue;
        this.NewWeapon = newWeapon;
    }
}
