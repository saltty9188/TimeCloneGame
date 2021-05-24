using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Prism : MovableObject
{
    #region Inspector fields
    [SerializeField] private Vector2[] outputDirections;
    #endregion

    
    void OnCollisionEnter2D(Collision2D other)
    {
        Projectile p = other.GetContact(0).collider.GetComponent<Projectile>();
        if(p && p.laser)
        {
            GameObject template = p.gameObject;

            foreach(Vector2 direction in outputDirections)
            {
                GameObject go = Instantiate(template, transform.position + new Vector3(direction.x, direction.y, 0) * 0.5f, Quaternion.identity);
                go.transform.GetChild(0).GetComponent<Light2D>().enabled = true;
                Projectile p1 = go.GetComponent<Projectile>();
                p1.direction = direction;
                p1.SetShooter(gameObject, true);
                p1.enabled = true;
                p1.GetComponent<Collider2D>().enabled = true;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
            }

            Destroy(other.GetContact(0).collider.gameObject);            
        }
    }


}
