using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    #region Public fields
    public static List<GameObject> targets;
    public static List<GameObject> enemies;
    #endregion

    #region Inspector fields
    [SerializeField] private FirstBossScript firstBossScript = null;
    [SerializeField] private SecondBossScript secondBossScript = null;
    #endregion
    void Awake()
    {
        targets = new List<GameObject>();
        enemies = new List<GameObject>();
    }

    public void CacheEnemyInfo()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject enemy = enemies[i];
            if(enemy == null)
            {
                enemies.Remove(enemy);
                i--;
            }
            else
            {
                Debug.Log(enemy.name);
                EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
                if(eb)
                {
                    eb.RecordingStarted();
                }
            }
        }
    }

    public void ResetEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject enemy = enemies[i];
            if(enemy == null)
            {
                enemies.Remove(enemy);
                i--;
            }
            else
            {
                enemy.SetActive(true);
                EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
                if(eb)
                {
                    eb.ResetEnemy();
                }

                EnemyStatus es = enemy.GetComponent<EnemyStatus>();
                if(es)
                {
                    es.ResetHealth();
                }

                Animator enemyAnimator = enemy.GetComponent<Animator>();
                if(enemyAnimator)
                {
                    enemyAnimator.SetTrigger("Reset");
                }
            }
        }
    }

    public void ResetCurrentBoss()
    {
        if(firstBossScript)
        {
            firstBossScript.ResetBoss();
        }
        else if(secondBossScript)
        {
            secondBossScript.ResetBoss();
        }
    }
}

    
