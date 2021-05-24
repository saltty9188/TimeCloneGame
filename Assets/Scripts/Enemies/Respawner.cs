using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private GameObject enemy;
    [SerializeField] private float respawnDelay;
    #endregion

    #region Private fields
    private EnemyBehaviour enemyBehaviour;
    private EnemyStatus enemyStatus;
    private Animator animator;
    private Animator enemyAnimator;
    private float respawnCountdown;
    private bool waitingToRespawn;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemy.transform.position = transform.position;
        enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.CacheInfo();
        enemyStatus = enemy.GetComponent<EnemyStatus>();
        enemyAnimator = enemy.GetComponent<Animator>();
        respawnCountdown = 0;
        waitingToRespawn = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(!enemy.activeSelf && respawnCountdown <= 0 && !waitingToRespawn)
        {
            respawnCountdown = respawnDelay;
            waitingToRespawn = true;
        }

        if(respawnCountdown > 0)
        {
            respawnCountdown -= Time.deltaTime;
        }
        else if(waitingToRespawn)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");          
        }
    }

    public void Respawn()
    {
        animator.ResetTrigger("Open");
        enemy.SetActive(true);
        enemy.transform.position = transform.position;
        enemyBehaviour.CacheInfo();
        enemyStatus.ResetHealth();
        if(enemyAnimator)
        {
            enemyAnimator.SetTrigger("Reset");
        }
        waitingToRespawn = false;
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}