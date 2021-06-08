using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject _player;
    #endregion  


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic("Intro");
    }

    public void ShowPlayer()
    {
        _player.SetActive(true);
    }
}
