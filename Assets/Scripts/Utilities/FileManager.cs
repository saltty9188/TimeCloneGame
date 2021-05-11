using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FileManager : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private FileViewer fileViewer;
    #endregion

    public void NewGame()
    {
        string fileName = "";
        if(tag == "NewFile")
        {
            int numFiles = transform.parent.childCount - 2;
            fileName = string.Format("File{0:D2}", numFiles + 1);    
        }
        else if(tag == "ExistingFile")
        {
            fileName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        }

        SaveData data = new SaveData(fileName, 1);
        SaveData.currentSaveFile = data;
        SaveSystem.SaveGame(data);
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        string fileName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        SaveData data = SaveSystem.LoadGame(fileName);
        SaveData.currentSaveFile = data;
        //Replace with level select
        //SceneManager.LoadScene(SaveData.currentSaveFile.levelIndex);
        fileViewer.OpenLevelSelect(gameObject);
    }
}
