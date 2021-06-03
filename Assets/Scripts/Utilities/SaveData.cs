using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region Public fields
    public static SaveData currentSaveFile;
    
    public string FileName
    {
        get{return _fileName;}
    }

    public int LevelIndex
    {
        get {return _levelIndex;}
        set
        {
            if(value > 14) value = 14;
            if(value < 1) value = 1;
            _levelIndex = value;
        }
    }
    #endregion

    #region Private fields
    private string _fileName;
    private int _levelIndex;
    #endregion

    public SaveData(string fileName, int levelIndex)
    {
        _fileName = fileName;
        _levelIndex = levelIndex;
        if(levelIndex > 14) this._levelIndex = 14;
        if(levelIndex < 1) this._levelIndex = 1;
    }
}
