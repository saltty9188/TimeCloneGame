using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] protected float knockBackDuration = 1f;
    [SerializeField] protected float knockBackSpeed = 0.5f;
    #endregion

    #region Protected fields
    protected Rigidbody2D rigidbody;
    protected float knockBackTime;
    protected Vector2 knockBackDirection;
    protected bool startActiveState;
    #endregion

    #region Private fields
    private Vector3 startPos;
    private Vector3 startScale;
    #endregion
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        knockBackTime = 0;
        startPos = transform.position;
        startScale = transform.localScale;
        startActiveState = gameObject.activeSelf;
        EnemyManager.enemies.Add(gameObject);
    }

    public void ReceiveKnockBack(Vector2 direction)
    {
        knockBackTime = knockBackDuration;
        knockBackDirection = direction;
    }

    public virtual void CacheInfo()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        startActiveState = gameObject.activeSelf;
    }

    public virtual void ResetEnemy()
    {
        gameObject.SetActive(startActiveState);
        transform.position = startPos;
        transform.localScale = startScale;
        if(GetComponent<DamageFlash>()) GetComponent<DamageFlash>().ResetShader();
    }
}
