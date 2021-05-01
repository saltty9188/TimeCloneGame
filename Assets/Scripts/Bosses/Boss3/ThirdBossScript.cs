using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBossScript : MonoBehaviour
{

    private float hitCooldown;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hitCooldown > 0)
        {
            spriteRenderer.color = Color.cyan;
            hitCooldown -= Time.deltaTime;
        } 
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void GetHit()
    {
        if(hitCooldown <= 0)
        {
            hitCooldown = 0.4f;
        }
        else
        {
            Debug.Log("Took Damage");
        }
    }

}
