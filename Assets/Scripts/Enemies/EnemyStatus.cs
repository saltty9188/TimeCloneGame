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
        flashScript.Flash();
        if(health < 1) Die();
        else if(knockBackDirection != Vector2.zero)
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
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if(healthBar != null) healthBar.value = (int) health;
    }
}
