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
    #endregion

    #region Private fields
    private Vector3 startPos;
    private Vector3 startScale;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        knockBackTime = 0;
        startPos = transform.position;
        startScale = transform.localScale;
        EnemyManager.enemies.Add(gameObject);
    }

    public void ReceiveKnockBack(Vector2 direction)
    {
        knockBackTime = knockBackDuration;
        knockBackDirection = direction;
    }

    public void RecordingStarted()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    public virtual void ResetEnemy()
    {
        transform.position = startPos;
        transform.localScale = startScale;
        GetComponent<DamageFlash>().ResetShader();
    }
}
