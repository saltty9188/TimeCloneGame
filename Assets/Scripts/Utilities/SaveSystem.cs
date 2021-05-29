using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(SaveData saveData)
    {
        string path = Application.persistentDataPath + "/" + saveData.FileName + ".sav";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static SaveData LoadGame(string fileName)
    {
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
