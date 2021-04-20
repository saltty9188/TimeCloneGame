using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossDamageTrigger : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private int touchDamage = 5;
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
                ps.TakeDamage(touchDamage, knockback);
            }
        }
    }
}
