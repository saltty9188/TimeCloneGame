using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The LevelSelectButton class is responsible for loading the selected level.
/// </summary>
public class LevelSelectButton : MonoBehaviour
{
    #region Private fields
    private static int _totalLevelNumber = 0;
    private int levelNumber;
    #endregion

    void Awake() 
    {
        if(name == "LevelSelectTemplate")
        {
            _totalLevelNumber = 0;
        }
        else
        {
            levelNumber = ++_totalLevelNumber;
        }
    }

    /// <summary>
    /// Loads the level represented by this button.
    /// </summary>
    public void LoadLevel()
    {
        SceneManager.LoadScene(levelNumber);
    }

    /// <summary>
    /// Resets the total level number.
    /// </summary>
    public static void ResetTotalLevelNumber()
    {
        _totalLevelNumber = 0;
    }
}
