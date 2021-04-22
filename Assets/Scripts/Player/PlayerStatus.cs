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
    #endregion

    #region Private fields
    private GameObject projectileParent;
    private DamageFlash flashScript;
    private PlayerMovement movementScript;
    private float health;
    private float damageCooldown;
    private float invincibiltyTimer;
    private bool collisionBelow = false;
    private bool collisionAbove = false;
    private Rigidbody2D aboveContact = null;
    private Rigidbody2D belowContact = null;
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
            if(health < maxHealth)
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
            flashScript.Flash();
            if(health < 1) Die();
            else 
            {
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
    }

    public void Die()
    {
        health = 0;
        if(tag == "Player")
        {
            Recorder r = GetComponent<Recorder>();
            r.ResetAllObjects();
            r.CancelRecording();
            DestroyAllProjectiles();
            if(enemyManager) 
            {
                enemyManager.ResetCurrentBoss();
                enemyManager.ResetEnemies();
            }
            Respawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Respawn()
    {
        health = maxHealth;
        UpdateUI();
        transform.position = CheckPoint.lastCheckpoint.transform.position;
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

    void OnCollisionStay2D(Collision2D other) 
    {
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach(ContactPoint2D contact in contacts)
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