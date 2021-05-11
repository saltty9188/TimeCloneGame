using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            int levelIndex = SceneManager.GetActiveScene().buildIndex;
            if(SaveData.currentSaveFile.levelIndex <= levelIndex)
            {
                SaveData.currentSaveFile.levelIndex = levelIndex + 1;
                SaveSystem.SaveGame(SaveData.currentSaveFile);
            }

            SceneManager.LoadScene(levelIndex + 1);
        }
    }
}
