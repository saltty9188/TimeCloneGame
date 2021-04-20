using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirstBossStatus : MonoBehaviour
{

    #region  Inspector fields
    [SerializeField] private float maxHealth = 500;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private Door exit;

    [SerializeField] private GameObject player;

    [SerializeField] private float weakPointExposeDuration = 0.15f;

    #endregion

    #region Private fields
    private float health;
    private float timer;

    private DamageFlash flashScript;
    #endregion

    void Start()
    {
        flashScript = GetComponent<DamageFlash>();
        timer = 0;
        health = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if(timer > 0) timer -= Time.deltaTime;
        else HideWeakPoint();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateUI();
        flashScript.Flash();
        if(health < 1) Die();
    }

    public void ExposeWeakPoint()
    {
        transform.GetChild(2).gameObject.SetActive(true);
        flashScript.Flash();
        timer = weakPointExposeDuration;
    }

    void HideWeakPoint()
    {
        transform.GetChild(2).gameObject.SetActive(false);
    }

    public void Die()
    {
        health = 0;
        Destroy(gameObject);
        healthBar.transform.parent.gameObject.SetActive(false);
        exit.AddActivation();
        if(player) player.GetComponent<Recorder>().CancelRecording();
    }

    public void ResetStatus()
    {
        health = maxHealth;
        UpdateUI();
        healthBar.transform.parent.gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if(healthBar != null) healthBar.value = health/maxHealth * 100.0f;
        if(healthText != null) healthText.text = ((int) health < 0 ? "0" : ((int) health).ToString());
    }
}
