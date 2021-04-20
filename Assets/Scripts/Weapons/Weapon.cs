using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] protected GameObject projectile;
    [SerializeField] private GameObject player;
    [SerializeField] protected float fireCooldown;
    #endregion

    #region Protected fields
    protected float accumulatedTime;
    #endregion

    #region Public fields
    //Check if this weapon is already being held
    public bool held;
    #endregion

    #region Private fields
    //Local position of weapon when picked up by Player
    private Vector3 pickUpPoint = new Vector3(1.58f, 0.17f, 0);
    private SpriteRenderer spriteRenderer;
    private Aim aimScript;
    private Vector3 initialSpawn;
    #endregion

    protected virtual void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        aimScript = player.transform.GetChild(0).gameObject.GetComponent<Aim>();
        accumulatedTime = 0;
        held = false;
        initialSpawn = transform.position;
    }

    void Update()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, spriteRenderer.bounds.size.x);

        if(!held)
        {
            //Check for collision
            foreach(Collider2D collider in colliders)
            {  
                if(collider.gameObject == player)
                {
                    aimScript.PickUpWeapon(this);
                    held = true;
                }
            }
        }
        if(accumulatedTime < fireCooldown) accumulatedTime += Time.deltaTime;
    }

    public void PickUp(GameObject holder)
    {
        transform.SetParent(holder.transform);
        transform.localPosition = pickUpPoint;
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public virtual GameObject Shoot(Quaternion rotation)
    {
        if(accumulatedTime >= fireCooldown)
        {
            GameObject go = Instantiate(projectile, transform.GetChild(0).position, rotation);
            go.layer = 9;
            Projectile p = go.GetComponent<Projectile>();
            p.direction = transform.parent.GetChild(1).position - transform.parent.position;
            p.SetShooter(transform.parent.parent.gameObject);
            accumulatedTime = 0;
            return go;
        }
        return null;
    }

    public virtual void ResetWeapon()
    {
        held = false;
        transform.parent = null;
        transform.position = initialSpawn;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.rotation = new Quaternion();
    }
}
