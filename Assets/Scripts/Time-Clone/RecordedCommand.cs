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
    public bool hadWeapon;
    public float raySwitchValue;
    #endregion

    public RecordedCommand(Vector2 movement, bool jumping, float aimAngle, bool shooting, float time, bool hadWeapon, float raySwitchValue)
    {
        this.movement = movement;
        this.jumping = jumping;
        this.aimAngle = aimAngle;
        this.shooting = shooting;
        this.time = time;
        this.hadWeapon = hadWeapon;
        this.raySwitchValue = raySwitchValue;
    }
}
