using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    #region Public fields
    public static EnemyManager instance;
    public static List<GameObject> targets;
    public static List<GameObject> enemies;
    #endregion

    #region Inspector fields
    [SerializeField] private FirstBossScript firstBossScript = null;
    [SerializeField] private SecondBossScript secondBossScript = null;
    [SerializeField] private ThirdBossScript thirdBossScript = null;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelay;
    #endregion
    void Awake()
    {
        instance = this;
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
                EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
                if(eb)
                {
                    eb.CacheInfo();
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
                //enemy.SetActive(true);

                Turret turret = enemy.GetComponent<Turret>();
                if(turret && turret.hasBase)
                {
                    enemy.transform.parent.gameObject.SetActive(true);
                }

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
        else if(thirdBossScript)
        {
            thirdBossScript.ResetBoss();
        }
    }

    public void SpawnExplosions(params Vector3[] positions)
    {
        StartCoroutine(Explosions(positions));
    }

    IEnumerator Explosions(Vector3[] positions)
    {
        foreach(Vector3 position in positions)
        {
            Instantiate(explosionPrefab, position, new Quaternion());
            yield return new WaitForSeconds(explosionDelay);
        }
    }
}

    
