using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RecordedCommand
{
    #region Public fields
    public Vector2 movement;
    public bool jumping;
    public float aimAngle;
    public bool shooting;
    public float time;
    public float raySwitchValue;
    public bool grabbing;
    public bool movingMirror;
    public float mirrorMoveValue;
    public GameObject newWeapon;
    #endregion

    public RecordedCommand(Vector2 movement, bool jumping, float aimAngle, bool shooting, float time, float raySwitchValue, bool grabbing, bool movingMirror, float mirrorMoveValue, GameObject newWeapon = null)
    {
        this.movement = movement;
        this.jumping = jumping;
        this.aimAngle = aimAngle;
        this.shooting = shooting;
        this.time = time;
        this.raySwitchValue = raySwitchValue;
        this.grabbing = grabbing;
        this.movingMirror = movingMirror;
        this.mirrorMoveValue = mirrorMoveValue;
        this.newWeapon = newWeapon;
    }
}
