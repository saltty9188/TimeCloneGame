using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The EnemyStatus class is responsible for performing actions related to the health of the enemy.
/// </summary>
public class EnemyStatus : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The max health of this enemy.")]
    [SerializeField] private float _maxHealth = 100;
    [Tooltip("The slider that will display the enemy's current health.")]
    [SerializeField] private Slider _healthBar;
    [Tooltip("The number of explosions that will spawn upon this enemy's defeat.")]
    [SerializeField] private int _numExplosions = 2;
    #endregion

    #region Private fields
    private DamageFlash _flashScript;
    private EnemyBehaviour _behaviour;
    private float _health;
    #endregion

    
    void Start()
    {
        _flashScript = GetComponent<DamageFlash>();
        _behaviour = GetComponent<EnemyBehaviour>();
        _health = _maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.minValue = 0;
    }

    
    void Update()
    {
        // Only show the health bar if the enemy has taken damage
        if (_health < _maxHealth)
        {
            _healthBar.gameObject.SetActive(true);
            // Don't flip the healthbar with the parent enemy
            if (transform.localScale.x < 0)
            {
                Vector3 temp = _healthBar.transform.localScale;
                temp.x = -Mathf.Abs(temp.x);
                _healthBar.transform.localScale = temp;
            }
            else
            {
                Vector3 temp = _healthBar.transform.localScale;
                temp.x = Mathf.Abs(temp.x);
                _healthBar.transform.localScale = temp;
            }
        }
        else
        {
            _healthBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Deals damage to the enemy and updates the UI.
    /// </summary>
    /// <remarks>
    /// Kills the enemy if health is at or below zero.
    /// </remarks>
    /// <param name="damage">The amount of damage to be taken.</param>
    /// <param name="knockBackDirection">The direction of the knock back to be taken, if left empty no knock back is taken.</param>
    public void TakeDamage(int damage, Vector2 knockBackDirection = new Vector2())
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
        // Only receive knock back if a direction was specified
        if(knockBackDirection != Vector2.zero)
        {
            _behaviour.ReceiveKnockBack(knockBackDirection.normalized);
        }
    }

    /// <summary>
    /// Resets the enemy back to max health and updates the UI.
    /// </summary>
    public void ResetHealth()
    {
        _health = _maxHealth;
        UpdateUI();
    }

    void Die()
    {
        SpiderBot sb = GetComponent<SpiderBot>();
        // Make SpiderBots drop their item before dying
        if(sb)
        {
            sb.DropObject();
        }
        _health = 0;
        CreateExplosions();
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if(_healthBar != null) _healthBar.value = (int) _health;
    }

    void CreateExplosions()
    {
        float explosionRadius = GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4.0f;
        Vector3[] positions = new Vector3[_numExplosions];
        for(int i = 0; i < _numExplosions; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            position.Normalize();
            positions[i] = (position * explosionRadius * Random.Range(0.0f, 1.0f)) + transform.position;
        }

        EnemyManager.Instance.SpawnExplosions(positions);
    }
}
