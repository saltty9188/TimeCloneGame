using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// The SaveSystem static class is responsible for managing SaveData.
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// Saves the given SaveData on disk.
    /// </summary>
    /// <remarks>
    /// The data is formatted using a BinaryFormatter before saving.
    /// Save location is determined by UnityEngine.Application.persistentDataPath.
    /// </remarks>
    /// <param name="saveData">The SaveData to be saved.</param>
    public static void SaveGame(SaveData saveData)
    {
        string path = Application.persistentDataPath + "/" + saveData.FileName + ".sav";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    /// <summary>
    /// Loads the save file at the given file name.
    /// </summary>
    /// <param name="fileName">The name of the file to be loaded with no file extensions.</param>
    /// <returns>The SaveData contained within the file.</returns>
    public static SaveData LoadGame(string fileName)
    {
        // strip .sav file extension to be safe
        if(fileName.Contains("*.sav"))
        {
            fileName.Remove(fileName.IndexOf("."));
        }

        string path = Application.persistentDataPath + "/" + fileName + ".sav";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = (SaveData) formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found at: " + path);
            return null;
        }
    }

    /// <summary>
    /// Returns an array containging the names of all of the files saved on disk.
    /// </summary>
    /// <returns>An array containging the names of all of the files saved on disk.</returns>
    public static string[] AllFileNames()
    {
        string[] nameAndPath = Directory.GetFiles(Application.persistentDataPath, "*.sav");
        string[] nameWithoutPath = new string[nameAndPath.Length];

        for(int i = 0; i < nameAndPath.Length; i++)
        {
            string longName = nameAndPath[i];
            longName = longName.Remove(0, Application.persistentDataPath.Length + 1);
            nameWithoutPath[i] = longName;
        }

        return nameWithoutPath;
    }
}
