using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    private static string filePath = Application.persistentDataPath + "/gameData.json";

    public static void SaveToFile(string json)
    {
        File.WriteAllText(filePath, json);
    }

    public static string LoadFromFile()
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        return null;
    }
}
