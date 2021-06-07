using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// The PlayerStatus class is responsible for performing actions related to the health of the player and time-clones.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The maximum health of the player.")]
    [SerializeField] private float _maxHealth = 100;
    [Tooltip("The slider that displays the player's current health.")]
    [SerializeField] private Slider _healthBar;
    [Tooltip("The text that displays the player's current health.")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [Tooltip("The amount of time the player is invincible for after taking a hit.")]
    [SerializeField] private float _invincibleTime = 0.3f;
    [Tooltip("The prefab for the clone death animation.")]
    [SerializeField] private GameObject _cloneDeathPrefab;
    #endregion

    #region Private fields
    private GameObject _projectileParent;
    private DamageFlash _flashScript;
    private PlayerMovement _movementScript;
    private Aim _aim;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private float _health;
    private float _damageCooldown;
    private float _invincibiltyTimer;
    private Weapon _startingWeapon;
    private bool _collisionBelow = false;
    private bool _collisionAbove = false;
    private Rigidbody2D _aboveContact = null;
    private Rigidbody2D _belowContact = null;
    private Coroutine _deathAnimation;
    #endregion

    void Start()
    {
        // Set up the empty game object that will handle fired projectiles
        _projectileParent = new GameObject(tag + " Projectiles");
        //add player to target list
        if(EnemyManager.Targets != null) EnemyManager.Targets.Add(gameObject);
        _damageCooldown = 0;
        _health = _maxHealth;
        _flashScript = GetComponent<DamageFlash>();
        _movementScript = GetComponent<PlayerMovement>();
        _aim = transform.GetChild(0).GetComponent<Aim>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _startingWeapon = null;
        _deathAnimation = null;
    }
    void Update()
    {
        // Clone health bar only
        if(_healthBar.transform.parent.gameObject.GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
        {
            // Only show the health bar if not at max health
            if(_health < _maxHealth)
            {
                _healthBar.gameObject.SetActive(true);
                 // For the clone health bar make sure it doesn't flip
                if(transform.localScale.x < 0)
                {
                    Vector3 temp = _healthBar.transform.localScale;
                    temp.x = -Mathf.Abs(temp.x);
                    _healthBar.transform.localScale = temp;
                }
                else
                {
                    Vector3 temp = _healthBar.transform.localScale;
                    temp.x = Mathf.Abs(temp.x);
                    _healthBar.transform.localScale = temp;
                }
            }
            else
            {
                _healthBar.gameObject.SetActive(false);
            }
        }

        // Regen health after the cooldown
        if(_damageCooldown > 0) _damageCooldown -= Time.deltaTime;
        else
        {
            if(_health < _maxHealth && _deathAnimation == null)
            {
                _health += 10 * Time.deltaTime;
                if(_health > _maxHealth) _health = _maxHealth;

                UpdateUI();
            }
        }

        // Get crushed
        if(_collisionAbove && _collisionBelow && ((_aboveContact && _aboveContact.velocity.y < 0) || (_belowContact && _belowContact.velocity.y > 0)))
        {
            Die();
        }

        if(_invincibiltyTimer > 0) _invincibiltyTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Causes the player to take damage.
    /// </summary>
    /// <param name="damage">The amount of damage to be taken.</param>
    /// <param name="knockBackDirection">The direction of knock back to be applied. Set to Vector2.zero if no knock back.</param>
    public void TakeDamage(int damage, Vector2 knockBackDirection = new Vector2())
    {
        // only take damage if the invincibily time is up
        if(_invincibiltyTimer <= 0)
        {
            _health -= damage;
            UpdateUI();
            if(_health < 1) 
            {
                Die();
                return;
            }
            AudioManager.Instance.PlaySFX("Hit");
            _flashScript.Flash();

            _damageCooldown = 3;
            _invincibiltyTimer = _invincibleTime;
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
                _movementScript.ReceiveKnockBack(knockBackDirection);
            }
        }
    }

    /// <summary>
    /// Kills the player and plays the death animation.
    /// </summary>
    /// <remarks>
    /// Respawns the player at the <see cref="CheckPoint">last checkpoint</see>.
    /// </remarks>
    public void Die()
    {
        _health = 0;
        // just in case they were stopped on a ladder before dying
        _movementScript.OffLadder();
        if(tag == "Player")
        {
            // only start the death animation once
            if(_deathAnimation == null)
            {
                AudioManager.Instance.PlaySFX("PlayerDeath");
                _animator.ResetTrigger("Respawn");
                _animator.SetBool("Jump", false);
                _movementScript.enabled = false;
                if(_aim.CurrentWeapon)
                {
                    _aim.CurrentWeapon.gameObject.SetActive(false);
                    _aim.DropWeapon(Vector3.zero);
                    _aim.gameObject.SetActive(false);
                }
                _deathAnimation = StartCoroutine(DeathAnimation());
            }
        }
        else
        {
            // instantiate the clone death animation then destroy the clone
            GameObject go = Instantiate(_cloneDeathPrefab, transform.position, new Quaternion());
            go.transform.localScale = transform.localScale;
            go.GetComponent<CloneDeath>().SetColor(GetComponent<SpriteRenderer>().color);
            Destroy(gameObject);
        }
    }

    IEnumerator DeathAnimation()
    {
        _animator.SetTrigger("Die");
        // Have player drop to the ground more quickly and not be able to be moved by other objects
        _rigidbody2D.gravityScale = 30;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // Wait one frame for animation to start
        yield return null;
        // wait for a bit after the animation is finished
        AnimationClip[] ac = _animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "Death")
            {
                animationTime = clip.length;
            }
        }
        _animator.ResetTrigger("Die");
        yield return new WaitForSeconds(animationTime + 1);

        Respawn();
        _deathAnimation = null;
    }   

    /// <summary>
    /// Respawns the player at the <see cref="CheckPoint">last checkpoint</see>.
    /// </summary>
    public void Respawn()
    {
        _animator.SetTrigger("Respawn");

        // Restore the rigidbody back to normal
        _rigidbody2D.gravityScale = 3;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Reset events and objects and cancel the recording
        Recorder r = GetComponent<Recorder>();
        r.CancelRecording(true);
        r.ResetAllEvents();
        PhysicsObject.ResetAllPhysics(true, true);

        DestroyAllProjectiles();
        GetComponent<PlayerController>().StopMovingMirrors();

        // Reset enemies and bosses
        if(EnemyManager.Instance) 
        {
            EnemyManager.Instance.ResetCurrentBoss();
            EnemyManager.Instance.ResetEnemies();
        }
        
        // Reset the weapon to the one used at the last check point
        if(_aim.CurrentWeapon != null) _aim.DropWeapon(Vector3.zero);
        if(WeaponManager.weapons != null) WeaponManager.ResetAllWeapons();
        if(_startingWeapon != null) 
        {
            _aim.gameObject.SetActive(true);
            _aim.PickUpWeapon(_startingWeapon);
        }
        
        _health = _maxHealth;
        UpdateUI();
        transform.position = CheckPoint.lastCheckpoint.transform.position;
        _movementScript.enabled = true;
    }

    void UpdateUI()
    {
        if(_healthBar != null) _healthBar.value = (int) _health;
        if(_healthText != null) _healthText.text = ((int) _health < 0 ? "0" : ((int) _health).ToString());
    }

    /// <summary>
    /// Adds the given projectile to the projectile parent so it can destroyed when needed.
    /// </summary>
    /// <param name="projectile">The projectile to be added.</param>
    public void AddProjectile(GameObject projectile)
    {
        projectile.transform.parent = _projectileParent.transform;
    }

    /// <summary>
    /// Destroys all of the projectiles fired by the player/time-clone.
    /// </summary>
    /// <param name="createNewParent">Whether or not to create a new projectile parent.</param>
    public void DestroyAllProjectiles(bool createNewParent = true)
    {
        Destroy(_projectileParent);
        if(createNewParent) _projectileParent = new GameObject(tag  + " Projectiles");
    }

    /// <summary>
    /// Sets the starting weapon for the player to re-equip upon their respawn.
    /// </summary>
    public void SetStartingWeapon()
    {
        _startingWeapon = _aim.CurrentWeapon;
    }

    // Check for above and below contacts for crusher deaths
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
                    _collisionBelow = true;
                    _belowContact = contact.collider.GetComponent<Rigidbody2D>();
                }
                else if(angle > 179.5f)
                {
                    _collisionAbove = true;
                    _aboveContact = contact.collider.GetComponent<Rigidbody2D>();
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D other) 
    {
        if(other.collider.GetComponent<Rigidbody2D>() == _aboveContact)
        {
            _aboveContact = null;
            _collisionAbove = false;
        }
        else if(other.collider.GetComponent<Rigidbody2D>() == _belowContact)
        {
            _belowContact = null;
            _collisionBelow = false;
        }
    }
}