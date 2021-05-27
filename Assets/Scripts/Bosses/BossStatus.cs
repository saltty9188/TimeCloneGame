using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossStatus : MonoBehaviour
{

    #region  Inspector fields
    [SerializeField] private float maxHealth = 500;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Door exit;
    [SerializeField] private GameObject player;
    #endregion

    #region Private fields
    private float health;
    private DamageFlash flashScript;
    private bool dead;
    #endregion
    
    void Start()
    {
        flashScript = GetComponent<DamageFlash>();
        health = maxHealth;
        UpdateUI();
        dead = false;
    }

    public void TakeDamage(int damage)
    {
       if(!dead)
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
       }
    }

    public void Die()
    {
        //Only run the death script once
        if(!dead)
        {
            dead = true;
            ThirdBossScript tbs = GetComponent<ThirdBossScript>();
            if(tbs)
            {
                tbs.SetDoorsClosed();
            }

            health = 0;
            healthBar.transform.parent.gameObject.SetActive(false);
            exit.AddActivation();
            if(player) player.GetComponent<Recorder>().CancelRecording();

            CreateExplosions();

            //Death animations
            switch(tag)
            {
                case "Boss1":
                    GetComponent<FirstBossScript>().DeathGibs();
                    Destroy(gameObject);
                    break;

                case "Boss2":
                case "Boss3":
                    GetComponent<Animator>().SetTrigger("Die");
                    break;
            }
        }

    }

    public void ResetStatus()
    {
        health = maxHealth;
        UpdateUI();
        healthBar.transform.parent.gameObject.SetActive(false);
    }

    void CreateExplosions()
    {
        float explosionRadius = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4.0f;
        Vector3[] positions = new Vector3[7];
        for(int i = 0; i < positions.Length; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            position.Normalize();
            positions[i] = (position * explosionRadius * Random.Range(0.0f, 1.0f)) + transform.position;
        }

        EnemyManager.instance.SpawnExplosions(positions);
    }

    void UpdateUI()
    {
        if(healthBar != null) healthBar.value = health/maxHealth * 100.0f;
        if(healthText != null) healthText.text = ((int) health < 0 ? "0" : ((int) health).ToString());
    }
}
