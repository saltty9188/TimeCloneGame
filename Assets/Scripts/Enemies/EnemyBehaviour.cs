using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnemyBehaviour class is the base class that holds common functions and fields for enemies.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] protected float _knockBackDuration = 1f;
    [SerializeField] protected float _knockBackSpeed = 0.5f;
    #endregion

    #region Protected fields
    /// <summary>
    /// The RigidBody2D attached to this enemy.
    /// </summary>
    protected Rigidbody2D Rigidbody;
    /// <summary>
    /// The timer that keeps track of how long the enemy has left being in the knock back state.
    /// </summary>
    protected float KnockBackTime;
    /// <summary>
    /// The direction of the knock back inflicted on this enemy.
    /// </summary>
    protected Vector2 KnockBackDirection;
    /// <value>Whether this enemy's GameObject was active or not when a time-clone recording started.</value>
    protected bool StartActiveState
    {
        get {return _startActiveState;}
    }
    #endregion

    #region Private fields
    private Vector3 _startPos;
    private Vector3 _startScale;
    private bool _startActiveState;
    #endregion
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        KnockBackTime = 0;
        _startPos = transform.position;
        _startScale = transform.localScale;
        _startActiveState = gameObject.activeSelf;
        EnemyManager.enemies.Add(gameObject);
    }

    /// <summary>
    /// Puts the enemy in the knock back state.
    /// </summary>
    /// <param name="direction">The direction for the knockback to be applied.</param>
    public void ReceiveKnockBack(Vector2 direction)
    {
        KnockBackTime = _knockBackDuration;
        KnockBackDirection = direction;
    }

    /// <summary>
    /// Caches the enemy's current position, scale, and active state.
    /// </summary>
    public virtual void CacheInfo()
    {
        _startPos = transform.position;
        _startScale = transform.localScale;
        _startActiveState = gameObject.activeSelf;
    }

    /// <summary>
    /// Resets the enemy to its cached position, scale and active state.
    /// </summary>
    public virtual void ResetEnemy()
    {
        gameObject.SetActive(_startActiveState);
        transform.position = _startPos;
        transform.localScale = _startScale;
        if(GetComponent<DamageFlash>()) GetComponent<DamageFlash>().ResetShader();
    }
}
