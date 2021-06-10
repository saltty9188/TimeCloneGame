using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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
            string[] allFileNames = SaveSystem.AllFileNames();
            int fileNum = -1;
            for(int i = 0; i < allFileNames.Length && fileNum == -1; i++)
            {
                string currentFile = allFileNames[i];
                // check that name follows normal conventions
                if(currentFile.Contains("File") && currentFile.Length >= 6)
                {
                    string currentFileNum = currentFile.Remove(currentFile.IndexOf(".")).Substring(4);
                    if(Int32.TryParse(currentFileNum, out int value))
                    {
                        // if there's a missing number (ie File01 -> File03) set the new file num to the missing number
                        if(value != i+1)
                        {
                            fileNum = i+1;
                        }
                    }
                }
            }
            // if no number was found set it to the next number
            if(fileNum == -1)
            {
                fileNum = allFileNames.Length + 1;
            }

            fileName = string.Format("File{0:D2}", fileNum);    
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

    /// <summary>
    /// Deletes the selected SaveData.
    /// </summary>
    public void DeleteGame()
    {
        string fileName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        SaveSystem.DeleteGame(fileName);
    }
}
