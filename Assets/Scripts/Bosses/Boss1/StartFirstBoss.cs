using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFirstBoss : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private Door doorBehind;

    [SerializeField] private FirstBossScript bossScript;

    [SerializeField] private GameObject bossUI;
    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            doorBehind.ResetEvent();
            bossScript.StartFight();
            bossUI.SetActive(true);  
        }
    }
}
