using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private EnemyManager enemyManager = null;
    [SerializeField] private float invincibleTime = 2f;
    [SerializeField] private GameObject cloneDeathPrefab;
    #endregion

    #region Private fields
    private GameObject projectileParent;
    private DamageFlash flashScript;
    private PlayerMovement movementScript;
    private Aim aim;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private float health;
    private float damageCooldown;
    private float invincibiltyTimer;
    private Weapon startingWeapon;
    private bool collisionBelow = false;
    private bool collisionAbove = false;
    private Rigidbody2D aboveContact = null;
    private Rigidbody2D belowContact = null;
    private Coroutine deathAnimation;
    #endregion

    void Start()
    {
        projectileParent = new GameObject(tag + " Projectiles");
        //add player to target list
        if(EnemyManager.targets != null) EnemyManager.targets.Add(gameObject);
        damageCooldown = 0;
        health = maxHealth;
        flashScript = GetComponent<DamageFlash>();
        movementScript = GetComponent<PlayerMovement>();
        aim = transform.GetChild(0).GetComponent<Aim>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startingWeapon = null;
        deathAnimation = null;
    }
    void Update()
    {
        if(healthBar.transform.parent.gameObject.GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
        {
            if(health < maxHealth)
            {
                healthBar.gameObject.SetActive(true);
                if(transform.localScale.x < 0)
                {
                    Vector3 temp = healthBar.transform.localScale;
                    temp.x = -Mathf.Abs(temp.x);
                    healthBar.transform.localScale = temp;
                }
                else
                {
                    Vector3 temp = healthBar.transform.localScale;
                    temp.x = Mathf.Abs(temp.x);
                    healthBar.transform.localScale = temp;
                }
            }
            else
            {
                healthBar.gameObject.SetActive(false);
            }
        }

        if(damageCooldown > 0) damageCooldown -= Time.deltaTime;
        else
        {
            if(health < maxHealth && deathAnimation == null)
            {
                health += 10 * Time.deltaTime;
                if(health > maxHealth) health = maxHealth;

                UpdateUI();
            }
        }

        if(collisionAbove && collisionBelow && ((aboveContact && aboveContact.velocity.y < 0) || (belowContact && belowContact.velocity.y > 0)))
        {
            Die();
        }

        if(invincibiltyTimer > 0) invincibiltyTimer -= Time.deltaTime;
    }

    public void TakeDamage(int damage, Vector2 knockBackDirection = new Vector2())
    {
       if(invincibiltyTimer <= 0)
       {
            health -= damage;
            UpdateUI();
            if(health < 1) 
            {
                Die();
                return;
            }
            AudioManager.instance.PlaySFX("Hit");
            flashScript.Flash();

            damageCooldown = 3;
            invincibiltyTimer = invincibleTime;
            if(knockBackDirection != Vector2.zero)
            {
                if(knockBackDirection.x == 0)
                {
                    if(transform.localScale.x > 0)
                    {
                        knockBackDirection.x = -1;
                    }
                    else
                    {
                        knockBackDirection.x = 1;
                    }
                }
                movementScript.ReceiveKnockBack(knockBackDirection);
            }
        }
    }

    public void Die()
    {
        health = 0;
        // just in case they were stopped on a ladder before dying
        movementScript.OffLadder();
        if(tag == "Player")
        {
            if(deathAnimation == null)
            {
                AudioManager.instance.PlaySFX("PlayerDeath");
                animator.ResetTrigger("Respawn");
                movementScript.enabled = false;
                if(aim.CurrentWeapon)
                {
                    aim.CurrentWeapon.gameObject.SetActive(false);
                    aim.DropWeapon();
                    aim.gameObject.SetActive(false);
                }
                deathAnimation = StartCoroutine(DeathAnimation());
            }
        }
        else
        {
            GameObject go = Instantiate(cloneDeathPrefab, transform.position, new Quaternion());
            go.transform.localScale = transform.localScale;
            go.GetComponent<CloneDeath>().SetColor(GetComponent<SpriteRenderer>().color);
            Destroy(gameObject);
        }
    }

    IEnumerator DeathAnimation()
    {
        animator.SetTrigger("Die");
        // Have player drop to the ground more quickly and not be able to be moved by other objects
        rigidbody2D.gravityScale = 30;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // Wait one frame for animation to start
        yield return null;
        AnimationClip[] ac = animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "Death")
            {
                animationTime = clip.length;
            }
        }
        animator.ResetTrigger("Die");
        yield return new WaitForSeconds(animationTime + 1);

        Respawn();
        deathAnimation = null;
        
    }   
    public void Respawn()
    {
        animator.SetTrigger("Respawn");
        rigidbody2D.gravityScale = 3;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        Recorder r = GetComponent<Recorder>();
        r.CancelRecording(true);
        r.ResetAllEvents();
        DestroyAllProjectiles();
        if(enemyManager) 
        {
            enemyManager.ResetCurrentBoss();
            enemyManager.ResetEnemies();
        }
        PhysicsObject.ResetAllPhysics(true, true);
        if(aim.CurrentWeapon != null) aim.DropWeapon();
        if(WeaponManager.weapons != null) WeaponManager.ResetAllWeapons();
        if(startingWeapon != null) 
        {
            aim.gameObject.SetActive(true);
            aim.PickUpWeapon(startingWeapon);
        }
        health = maxHealth;
        UpdateUI();
        transform.position = CheckPoint.lastCheckpoint.transform.position;
        movementScript.enabled = true;
    }

    void UpdateUI()
    {
        if(healthBar != null) healthBar.value = (int) health;
        if(healthText != null) healthText.text = ((int) health < 0 ? "0" : ((int) health).ToString());
    }

    
    public void AddProjectile(GameObject projectile)
    {
        projectile.transform.parent = projectileParent.transform;
    }

    public void DestroyAllProjectiles(bool createNewParent = true)
    {
        Destroy(projectileParent);
        if(createNewParent) projectileParent = new GameObject(tag  + " Projectiles");
    }

    public void SetStartingWeapon()
    {
        startingWeapon = aim.CurrentWeapon;
    }

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
        {
            if(contact.collider.tag != "Player" && contact.collider.tag != "Clone")
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if(angle < 0.5f)
                {
                    collisionBelow = true;
                    belowContact = contact.collider.GetComponent<Rigidbody2D>();
                }
                else if(angle > 179.5f)
                {
                    collisionAbove = true;
                    aboveContact = contact.collider.GetComponent<Rigidbody2D>();
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D other) 
    {
        if(other.collider.GetComponent<Rigidbody2D>() == aboveContact)
        {
            aboveContact = null;
            collisionAbove = false;
        }
        else if(other.collider.GetComponent<Rigidbody2D>() == belowContact)
        {
            belowContact = null;
            collisionBelow = false;
        }
    }
}