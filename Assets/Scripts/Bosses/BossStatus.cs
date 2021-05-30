using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The BossStatus class is responsible for performing actions related to the health of the boss.
/// </summary>
public class BossStatus : MonoBehaviour
{

    #region Inspector fields
    [Tooltip("The maximum health value for the boss fight.")]
    [SerializeField] private float _maxHealth = 500;
    [Tooltip("The slider that will display the boss's current health.")]
    [SerializeField] private Slider _healthBar;
    [Tooltip("The text element that is inside the health bar to display the boss's current health.")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [Tooltip("The exit of the arena that will be open upon the boss's death.")]
    [SerializeField] private Door _exit;
    [Tooltip("A reference to the player that will be used to cancel the recording if applicable.")]
    [SerializeField] private GameObject _player;
    #endregion

    #region Private fields
    private float _health;
    private DamageFlash _flashScript;
    private bool _dead;
    #endregion
    
    void Start()
    {
        _flashScript = GetComponent<DamageFlash>();
        _health = _maxHealth;
        UpdateUI();
        _dead = false;
    }

    /// <summary>
    /// Deals damage to the boss and updates the UI.
    /// </summary>
    /// <remarks>
    /// Kills the boss if health is at or below zero.
    /// </remarks>
    /// <param name="damage">The amount of damage to be taken.</param>
    public void TakeDamage(int damage)
    {
        if(!_dead)
        {
            _health -= damage;
            UpdateUI();
            if(_health < 1) 
            {
                Die();
                return;
            }
            AudioManager.Instance.PlaySFX("Hit");
            _flashScript.Flash();
        }
    }

    void Die()
    {
        //Only run the death script once
        if(!_dead)
        {
            _dead = true;
            ThirdBossScript tbs = GetComponent<ThirdBossScript>();
            if(tbs)
            {
                tbs.SetDoorsClosed();
            }

            _health = 0;
            _healthBar.transform.parent.gameObject.SetActive(false);
            _exit.AddActivation();
            if(_player) _player.GetComponent<Recorder>().CancelRecording();

            CreateExplosions();

            //Death animations
            switch(tag)
            {
                case "Boss1":
                    GetComponent<FirstBossScript>().DeathPieces();
                    Destroy(gameObject);
                    break;

                case "Boss2":
                case "Boss3":
                    GetComponent<Animator>().SetTrigger("Die");
                    break;
            }
        }

    }

    /// <summary>
    /// Resets the boss to full health and updates the UI accordingly.
    /// </summary>
    /// <remarks>
    /// Also hides the UI.
    /// </remarks>
    public void ResetStatus()
    {
        _health = _maxHealth;
        UpdateUI();
        _healthBar.transform.parent.gameObject.SetActive(false);
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
        if(_healthBar != null) _healthBar.value = _health/_maxHealth * 100.0f;
        if(_healthText != null) _healthText.text = ((int) _health < 0 ? "0" : ((int) _health).ToString());
    }
}
