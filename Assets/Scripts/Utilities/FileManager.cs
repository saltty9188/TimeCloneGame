using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// The FileManager class is responsible for maintaining exisiting SaveData.
/// </summary>
/// <remarks>
/// Can be used to create new files, overwrite existing files, or load existing files.
/// This script is attached to buttons in FileViewer and gets its information on SaveData from there.
/// </remarks>
public class FileManager : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The file viewer this game object is a grandchild of.")]
    [SerializeField] private FileViewer _fileViewer;
    #endregion

    /// <summary>
    /// Creates new SaveData.
    /// </summary>
    /// <remarks>
    /// Overwrites an existing file if the button that called this function already held a file.
    /// </remarks>
    public void NewGame()
    {
        string fileName = "";
        // Create new file with new name
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
        // Load the first level
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the selected SaveData.
    /// </summary>
    public void LoadGame()
    {
        string fileName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        SaveData data = SaveSystem.LoadGame(fileName);
        SaveData.currentSaveFile = data;
        _fileViewer.OpenLevelSelect(gameObject);
    }
}
