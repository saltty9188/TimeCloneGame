using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBossScript : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private float cooldownTime = 0.3f;
    [SerializeField] private float ventSwapTime = 20;
    [SerializeField] private Door topDoor;
    [SerializeField] private Door bottomDoor;
    [SerializeField] private Door[] backDoors;
    #endregion

    #region Private fields
    private BossStatus status;
    private SpriteRenderer spriteRenderer;
    private bool inFight;
    private float hitCooldown;
    private bool topVulnerable;
    private float ventSwapCooldown;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponent<BossStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitialValues();
    }

    // Update is called once per frame
    void Update()
    {
        if(inFight)
        {
            //shoot

            if(hitCooldown > 0)
            {
                spriteRenderer.color = Color.cyan;
                hitCooldown -= Time.deltaTime;
            } 
            else
            {
                spriteRenderer.color = Color.white;
            }

            if(ventSwapCooldown > 0)
            {
                ventSwapCooldown -= Time.deltaTime;
            }
            else
            {
                ventSwapCooldown = ventSwapTime;
                topVulnerable = !topVulnerable;
                if(topVulnerable)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                    transform.GetChild(2).gameObject.SetActive(false);
                    bottomDoor.AddActivation();
                    topDoor.RemoveActivation();
                }
                else
                {
                    transform.GetChild(2).gameObject.SetActive(true);
                    transform.GetChild(1).gameObject.SetActive(false);
                    topDoor.AddActivation();
                    bottomDoor.RemoveActivation();
                }
            }
        }
    }

    public void StartFight()
    {
        inFight = true;
        Debug.Log("started");

    }

    public void ResetBoss()
    {
        status.ResetStatus();
        hitCooldown = 0;
        InitialValues();
        //TODO: shoot time reset
    }

    void InitialValues()
    {
        ventSwapCooldown = ventSwapTime;
        topVulnerable = true;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
        bottomDoor.ResetEvent();
        bottomDoor.AddActivation();
        topDoor.ResetEvent();
        topDoor.RemoveActivation();

        inFight = false;
    }

    public void GetHit(int damage)
    {
        if(hitCooldown > 0)
        {
            status.TakeDamage(damage/2);
        }
    }

    public void MakeVulnerable()
    {
        hitCooldown = cooldownTime;
    }

    public void SetDoorsClosed()
    {
        topDoor.ResetAndTurnOff();
        bottomDoor.ResetAndTurnOff();
        foreach(Door door in backDoors)
        {
            door.ResetAndTurnOff();
        }
    }
}
