using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteCommands : MonoBehaviour
{
    #region Private fields
    private PlayerMovement playerMovement;
    private Aim aim;
    private List<RecordedCommand> recordedCommands;
    private int commandIndex;
    private float playbackTime;
    private bool unstable; 
    #endregion

    void Awake()
    {
        recordedCommands = null;
        commandIndex = -1;
        playerMovement = GetComponent<PlayerMovement>();
        aim = transform.GetChild(0).GetComponent<Aim>();
        playbackTime = 0;
    }

    void FixedUpdate()
    {
        if(commandIndex >= 0 && commandIndex < recordedCommands.Count)
        {
            RecordedCommand rc = recordedCommands[commandIndex];
            if(rc.time >= playbackTime)
            {
                //Remove weapon if player didn't have it yet
                if(aim.CurrentWeapon) aim.CurrentWeapon.gameObject.SetActive(rc.hadWeapon);
                playerMovement.move(rc.movement, rc.jumping);

                if(rc.raySwitchValue > 0)
                {
                    aim.NextRayType();
                }
                else if (rc.raySwitchValue < 0)
                {
                    aim.PrevRayType();
                }
                aim.CloneRotate(rc.aimAngle, rc.shooting);
                playbackTime += Time.fixedDeltaTime;
            }
            commandIndex++;
        }
        else
        {
            //Stop moving clone once commands end
            playerMovement.move(Vector2.zero, false);
            RecordedCommand rc = recordedCommands[commandIndex - 1];
            aim.CloneRotate(rc.aimAngle, rc.shooting);
        }

        if(playbackTime < 1.0f) Physics2D.IgnoreLayerCollision(8, 8, true);
        else Physics2D.IgnoreLayerCollision(8, 8, false);
    }

    public void SetUnstable(bool unstable)
    {
        this.unstable = unstable;
        if(unstable)
        {
            GetComponent<DamageFlash>().SetBaseColour(new Color(1, 0, 0, 0.59f));
        }
    }

    public void SetCommands(List<RecordedCommand> commands)
    {
        recordedCommands = commands;
        commandIndex = 0;
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(unstable && other.GetContact(0).collider.tag == "Player")
        {
            other.GetContact(0).collider.GetComponent<PlayerStatus>().Die();
        }
    }
}
