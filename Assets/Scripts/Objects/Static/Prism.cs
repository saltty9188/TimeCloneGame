using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The Prism class splits lasers that collide with it into multiple lasers.
/// </summary>
public class Prism : MovableObject
{
    #region Inspector fields
    [Tooltip("The directions the split lasers will be fired in.")]
    [SerializeField] private Vector2[] _outputDirections;
    #endregion

    // If a laser collides with this prism split it into multiple lasers that are fired in the output directions
    void OnCollisionEnter2D(Collision2D other)
    {
        Projectile p = other.GetContact(0).collider.GetComponent<Projectile>();
        if(p && p.laser)
        {
            GameObject template = p.gameObject;

            foreach(Vector2 direction in _outputDirections)
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

            AudioManager.Instance.PlaySFX("PrismHit");

            // Destroy the original
            Destroy(other.GetContact(0).collider.gameObject);            
        }
    }


}
