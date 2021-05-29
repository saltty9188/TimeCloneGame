using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region Public fields
    public static SaveData currentSaveFile;
    public string fileName;
    public int levelIndex;
    #endregion

    public SaveData(string fileName, int levelIndex)
    {
        this.fileName = fileName;
        this.levelIndex = levelIndex;
        if(levelIndex > 14) this.levelIndex = 14;
    }
}
