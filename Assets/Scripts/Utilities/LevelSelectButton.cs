using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public static int totalLevelNumber = 0;

    private int levelNumber;

    void Awake() 
    {
        if(name == "LevelSelectTemplate")
        {
            totalLevelNumber = 0;
        }
        else
        {
            levelNumber = ++totalLevelNumber;
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelNumber);
    }
}
