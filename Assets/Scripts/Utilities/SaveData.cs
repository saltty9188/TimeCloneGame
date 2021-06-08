

/// <summary>
/// The SaveData class is a simple serializable data class for saving the player's progress.
/// </summary>
[System.Serializable]
public class SaveData
{
    #region Public fields
    /// <summary>
    /// Static reference to the currently loaded SaveData.
    /// </summary>
    public static SaveData currentSaveFile;
    
    /// <value>The name of this file</value>
    public string FileName
    {
        get{return _fileName;}
    }

    /// <summary>
    /// The number of the latest unlocked level.
    /// </summary>
    public int LevelIndex
    {
        get {return _levelIndex;}
        set
        {
            // there are only 14 levels in the game 
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

    /// <summary>
    /// Creates a new SaveData containing the player's latest unlocked level.
    /// </summary>
    /// <param name="fileName">The name of the file for this SaveData.</param>
    /// <param name="levelIndex">The number of the latest unlocked level.</param>
    public SaveData(string fileName, int levelIndex)
    {
        _fileName = fileName;
        _levelIndex = levelIndex;
        // there are only 14 levels in the game 
        if(levelIndex > 14) this._levelIndex = 14;
        if(levelIndex < 1) this._levelIndex = 1;
    }
}
