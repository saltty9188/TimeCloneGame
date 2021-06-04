using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Respawner class respawns its set enemy.
/// </summary>
public class Respawner : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The enemy to be respawned.")]
    [SerializeField] private GameObject _enemy;
    [Tooltip("The amount of time after death before respawn.")]
    [SerializeField] private float _respawnDelay = 10;
    #endregion

    #region Private fields
    private EnemyBehaviour _enemyBehaviour;
    private EnemyStatus _enemyStatus;
    private Animator _animator;
    private Animator _enemyAnimator;
    private float _respawnCountdown;
    private bool _waitingToRespawn;
    #endregion

    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemy.transform.position = transform.position;
        _enemyBehaviour = _enemy.GetComponent<EnemyBehaviour>();
        _enemyBehaviour.CacheInfo();
        _enemyStatus = _enemy.GetComponent<EnemyStatus>();
        _enemyAnimator = _enemy.GetComponent<Animator>();
        _respawnCountdown = 0;
        _waitingToRespawn = false;
    }

    void Update()
    {
        // Countdown if the enemy is dead
        if(!_enemy.activeSelf && _respawnCountdown <= 0 && !_waitingToRespawn)
        {
            _respawnCountdown = _respawnDelay;
            _waitingToRespawn = true;
        }

        if(_respawnCountdown > 0)
        {
            _respawnCountdown -= Time.deltaTime;
        }
        else if(_waitingToRespawn)
        {
            _animator.ResetTrigger("Close");
            _animator.SetTrigger("Open");          
        }
    }

    /// <summary>
    /// Respawns the enemy attached to this Respawner.
    /// </summary>
    public void Respawn()
    {
        _animator.ResetTrigger("Open");
        _enemy.SetActive(true);
        _enemy.transform.position = transform.position;
        _enemyBehaviour.CacheInfo();
        _enemyStatus.ResetHealth();
        if(_enemyAnimator)
        {
            _enemyAnimator.SetTrigger("Reset");
        }
        _waitingToRespawn = false;
    }

    /// <summary>
    /// Starts the close animation.
    /// </summary>
    public void Close()
    {
        _animator.SetTrigger("Close");
    }
}
