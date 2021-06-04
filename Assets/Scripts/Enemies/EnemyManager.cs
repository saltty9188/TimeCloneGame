using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnemyManager singleton class manages enemies and keeps track of their targets.
/// </summary>
public class EnemyManager : MonoBehaviour
{

    #region Public fields
    /// <summary>
    /// The static reference to the EnemyManager singleton.
    /// </summary>
    public static EnemyManager Instance;
    /// <summary>
    /// A list containing the player and all time-clones that enemies can target.
    /// </summary>
    public static List<GameObject> Targets;
    /// <summary>
    /// A list containing every enemy in this scene.
    /// </summary>
    public static List<GameObject> Enemies;
    #endregion

    #region Inspector fields
    [Tooltip("The script for the first boss if it is present in this level.")]
    [SerializeField] private FirstBossScript _firstBossScript = null;
    [Tooltip("The script for the second boss if it is present in this level.")]
    [SerializeField] private SecondBossScript _secondBossScript = null;
    [Tooltip("The script for the third boss if it is present in this level.")]
    [SerializeField] private ThirdBossScript _thirdBossScript = null;
    [Tooltip("A prefab for the explosion graphic.")]
    [SerializeField] private GameObject _explosionPrefab;
    [Tooltip("The delay between each spawned explosion.")]
    [SerializeField] private float _explosionDelay = 0.4f;
    #endregion

    void Awake()
    {
        Instance = this;
        Targets = new List<GameObject>();
        Enemies = new List<GameObject>();
    }

    /// <summary>
    /// Calls <see cref="EnemyBehaviour.CacheInfo">CacheInfo</see> on every EnemyBehaviour within the Enemies list.
    /// </summary>
    public void CacheEnemyInfo()
    {
        for(int i = 0; i < Enemies.Count; i++)
        {
            GameObject enemy = Enemies[i];
            // Remove the enemy from the list if it's been destroyed
            if(enemy == null)
            {
                Enemies.Remove(enemy);
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

    /// <summary>
    /// Resets the state of all of the enemies within the Enemies list.
    /// </summary>
    public void ResetEnemies()
    {
        for(int i = 0; i < Enemies.Count; i++)
        {
            GameObject enemy = Enemies[i];
            // Remove the enemy from the list if it's been destroyed
            if(enemy == null)
            {
                Enemies.Remove(enemy);
                i--;
            }
            else
            {

                Turret turret = enemy.GetComponent<Turret>();
                if(turret && turret.HasBase)
                {
                    enemy.transform.parent.gameObject.SetActive(true);
                }

                EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
                // Reset location, scale, active state
                if(eb)
                {
                    eb.ResetEnemy();
                }

                EnemyStatus es = enemy.GetComponent<EnemyStatus>();
                // reset health
                if(es)
                {
                    es.ResetHealth();
                }

                Animator enemyAnimator = enemy.GetComponent<Animator>();
                // Reset animations where appropriate
                if(enemyAnimator)
                {
                    enemyAnimator.SetTrigger("Reset");
                }
            }
        }
    }

    /// <summary>
    /// Resets the state of the boss present in the current scene, if any.
    /// </summary>
    public void ResetCurrentBoss()
    {
        if(_firstBossScript)
        {
            _firstBossScript.ResetBoss();
        }
        else if(_secondBossScript)
        {
            _secondBossScript.ResetBoss();
        }
        else if(_thirdBossScript)
        {
            _thirdBossScript.ResetBoss();
        }
    }

    /// <summary>
    /// Spawns explosions at the locations listed.
    /// </summary>
    /// <param name="positions">The positions for the explosions to be spawned.</param>
    public void SpawnExplosions(Vector3[] positions)
    {
        StartCoroutine(Explosions(positions));
    }

    IEnumerator Explosions(Vector3[] positions)
    {
        foreach(Vector3 position in positions)
        {
            Instantiate(_explosionPrefab, position, new Quaternion());
            yield return new WaitForSeconds(_explosionDelay);
        }
    }
}

    
