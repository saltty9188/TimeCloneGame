using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private float speed;

    [SerializeField] public int damage;

    [SerializeField] public float noHitTime = 0.1f;
    #endregion

    #region Public fields
    public Vector2 direction;
    public bool laser;
    #endregion

    #region Private fields
    private BoxCollider2D boxCollider;

    private Rigidbody2D rigidbody2D;
    private GameObject shooter;
    private float aliveTime;
    private bool pastShooter;
    private bool ignoringShooter;
    private float cooldown;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        direction = new Vector2(1, 0).normalized;
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        aliveTime = 0;
        pastShooter = false;
        ignoringShooter = false;
        cooldown = 0;
    }

    protected virtual void Start()
    {
        Redirect(direction);
        IgnoreShooterCollision(true);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (aliveTime < noHitTime) aliveTime += Time.deltaTime;
        else 
        {
            if(!pastShooter && !ignoringShooter)
            {
                IgnoreShooterCollision(false);
                pastShooter = true;
            }
        }

        if(cooldown > 0) cooldown -= Time.deltaTime;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        //Destroy projectile if its off camera
        if (screenPos.x < -0.5f || screenPos.x > 1.5f || screenPos.y < -0.5f || screenPos.y > 1.5f)
        {
            GameObject.Destroy(gameObject);
        }

        //Catch collision bugs
        if (rigidbody2D.velocity == Vector2.zero) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D collider = other.GetContact(0).collider;

        //evaluate player collision first
        if (collider.tag == "Clone" || collider.tag == "Player")
        {
            collider.GetComponent<PlayerStatus>().TakeDamage(damage);       
        }
        else if (collider.tag == "Target")
        {
            Target t = collider.GetComponent<Target>();
            if (t)
            {
                t.Activate();
            }
        }
        else if (collider.tag == "WeakPoint")
        {
            if (collider.transform.parent && collider.transform.parent.tag == "Boss1")
            {
                FirstBossStatus bs = collider.transform.parent.GetComponent<FirstBossStatus>();
                if (bs)
                {
                    bs.TakeDamage(damage);
                }
            }
            else if(collider.transform.parent && collider.transform.parent.tag == "EnemyInvuln")
            {
                EnemyStatus es = collider.transform.parent.GetComponent<EnemyStatus>();
                if(es && collider.transform.parent.gameObject != shooter)
                {
                    es.TakeDamage(damage);
                }
            }
        }
        else if(collider.tag == "Boss2")
        {
            SecondBossStatus sbs = collider.GetComponent<SecondBossStatus>();
            if(sbs)
            {
                sbs.TakeDamage(damage);
            }
        }
        else if(collider.tag == "Enemy")
        {
            EnemyStatus es = collider.GetComponent<EnemyStatus>();
            if(es)
            {
                es.TakeDamage(damage, -other.GetContact(0).normal);
            }
        }
        else
        {
            if (laser && collider.tag == "Reflective")
            {
                //ricochete
                Redirect(Vector2.Reflect(direction, other.contacts[0].normal));
                cooldown = 0.1f;
                return;
            }
        }

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (laser && other.GetContact(0).collider.tag == "Reflective" && cooldown <= 0) // && reflective surface
        {
            //ricochete
            Redirect(Vector2.Reflect(direction, other.contacts[0].normal));
            cooldown = 0.1f;
        }
    }

    void IgnoreShooterCollision(bool ignore)
    {
        if (shooter != null)
        {
            Collider2D[] colliders = shooter.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), ignore);
            }
        }
    }

    public void IgnoreCollision(GameObject objectToIgnore)
    {
        Physics2D.IgnoreCollision(objectToIgnore.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());
    }

    public void SetShooter(GameObject shooter, bool ignoreShooter = false)
    {
        this.shooter = shooter;
        this.ignoringShooter = ignoreShooter;
    }

    public void Redirect(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        float newRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float offset = (direction.x < 0 ? 180 : 0);
        transform.rotation = Quaternion.Euler(0, 0, newRotation + offset);

        rigidbody2D.velocity = direction * speed;
    }
}
