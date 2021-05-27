using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatus : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private Slider healthBar;
    [SerializeField] private int numExplosions;
    #endregion

    #region Private fields
    private DamageFlash flashScript;
    private EnemyBehaviour behaviour;
    private float health;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        flashScript = GetComponent<DamageFlash>();
        behaviour = GetComponent<EnemyBehaviour>();
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHealth)
        {
            healthBar.gameObject.SetActive(true);
            if (transform.localScale.x < 0)
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

    public void TakeDamage(int damage, Vector2 knockBackDirection = new Vector2())
    {
        health -= damage;
        UpdateUI();
        if(health < 1) 
        {   
            Die();
            return;
        }
        AudioManager.instance.PlaySFX("Hit");
        flashScript.Flash();
        if(knockBackDirection != Vector2.zero)
        {
            behaviour.ReceiveKnockBack(knockBackDirection.normalized);
        }
    }

    public void ResetHealth()
    {
        health = maxHealth;
        UpdateUI();
    }

    public void Die()
    {
        SpiderBot sb = GetComponent<SpiderBot>();
        if(sb)
        {
            sb.DropObject();
        }
        health = 0;
        CreateExplosions();
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if(healthBar != null) healthBar.value = (int) health;
    }

    void CreateExplosions()
    {
        float explosionRadius = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4.0f;
        Vector3[] positions = new Vector3[numExplosions];
        for(int i = 0; i < numExplosions; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            position.Normalize();
            positions[i] = (position * explosionRadius * Random.Range(0.0f, 1.0f)) + transform.position;
        }

        EnemyManager.instance.SpawnExplosions(positions);
    }
}
