using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject player;
    #endregion  


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic("Intro");
    }

    public void ShowPlayer()
    {
        player.SetActive(true);
    }
}
