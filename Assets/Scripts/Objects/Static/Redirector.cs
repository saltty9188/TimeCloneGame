using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirector : MovableObject
{
    void OnCollisionEnter2D(Collision2D other)
    {
        Projectile p = other.GetContact(0).collider.GetComponent<Projectile>();
        if(p && p.laser)
        {
            GameObject template = p.gameObject;

            Vector2 direction = transform.GetChild(1).position - transform.position;
            direction.Normalize();

            GameObject go = Instantiate(template, transform.GetChild(1).position, Quaternion.identity);
            Projectile p1 = go.GetComponent<Projectile>();
            p1.direction = direction;
            p1.SetShooter(gameObject, true);
            p1.enabled = true;
            p1.GetComponent<Collider2D>().enabled = true;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), go.GetComponent<Collider2D>());      

            Destroy(other.GetContact(0).collider.gameObject);            
        }
    }
}
