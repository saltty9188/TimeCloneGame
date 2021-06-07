using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The Redirector class redirects lasers that collide with it.
/// </summary>
public class Redirector : MovableObject
{
    // Redirect incoming lasers to be fired out of the fire point.
    void OnCollisionEnter2D(Collision2D other)
    {
        Projectile p = other.GetContact(0).collider.GetComponent<Projectile>();
        if(p && p.laser)
        {
            GameObject template = p.gameObject;

            Vector2 direction = transform.GetChild(0).position - transform.position;
            direction.Normalize();

            GameObject go = Instantiate(template, transform.GetChild(0).position, Quaternion.identity);
            go.transform.GetChild(0).GetComponent<Light2D>().enabled = true;
            Projectile p1 = go.GetComponent<Projectile>();
            p1.direction = direction;
            p1.SetShooter(gameObject, true);
            p1.enabled = true;
            p1.GetComponent<Collider2D>().enabled = true;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), go.GetComponent<Collider2D>());      
            AudioManager.Instance.PlaySFX("PrismHit");
            
            Destroy(other.GetContact(0).collider.gameObject);            
        }
    }
}
