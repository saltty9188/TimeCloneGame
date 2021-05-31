using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossDamageTrigger class is a trigger that causes the player to take damage when they touch the second boss.
/// </summary>
public class SecondBossDamageTrigger : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("How much damage will be done to the player.")]
    [SerializeField] private int _touchDamage = 5;
    #endregion
    
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D>();
            Vector2 knockback = new Vector2(0, 1);
            if(rb.velocity.x > 0)
            {
                knockback.x = 1;
            }
            else if(rb.velocity.x < 0)
            {
                knockback.x = -1;
            }

            PlayerStatus ps = other.GetComponent<PlayerStatus>();
            if(ps)
            {
                ps.TakeDamage(_touchDamage, knockback);
            }
        }
    }
}
