using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Weapon : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] protected GameObject projectile;
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
    protected Vector3 pickUpPoint = new Vector3(1.428f, 0.138f, 0);
    private SpriteRenderer spriteRenderer;
    private Aim aimScript;
    private Light2D light;
    private Vector3 initialSpawn;
    private bool justDropped = false;
    #endregion

    protected virtual void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        accumulatedTime = 0;
        held = false;
        initialSpawn = transform.position;
        if(transform.childCount > 1) light = transform.GetChild(1).GetComponent<Light2D>();
    }

    void Start()
    {
        WeaponManager.weapons.Add(this);
    }

    void Update()
    {
        if(accumulatedTime < fireCooldown) accumulatedTime += Time.deltaTime;

        if(light && accumulatedTime >= 0.1f) light.gameObject.SetActive(false);
    }

    public void PickUp(GameObject holder)
    {
        transform.SetParent(holder.transform);
        held = true;
        transform.localPosition = pickUpPoint;
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void Drop(GameObject oldHolder)
    {
        transform.SetParent(null);
        transform.rotation = new Quaternion();
        transform.position = oldHolder.transform.position;
        held = false;
        justDropped = true;
    }

    public virtual GameObject Shoot(Quaternion rotation)
    {
        if(accumulatedTime >= fireCooldown)
        {
            GameObject go = Instantiate(projectile, transform.GetChild(0).position, rotation);
            go.layer = 9;
            Projectile p = go.GetComponent<Projectile>();
            p.direction = transform.parent.GetChild(0).position - transform.parent.position;
            p.SetShooter(transform.parent.parent.gameObject);
            accumulatedTime = 0;

            // Muzzle flash
            if(light) light.gameObject.SetActive(true);
            return go;
        }
        return null;
    }

    public void SetDefaultPosition()
    {
        initialSpawn = transform.position;
    }

    public virtual void ResetWeapon()
    {
        held = false;
        justDropped = false;
        transform.parent = null;
        transform.position = initialSpawn;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.rotation = new Quaternion();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(!held && !justDropped && other.gameObject.tag == "Player")
        {
            aimScript = other.gameObject.transform.GetChild(0).GetComponent<Aim>();
            aimScript.PickUpWeapon(this);
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Player" && !held)
        {
            justDropped = false;
        }
    }
}
