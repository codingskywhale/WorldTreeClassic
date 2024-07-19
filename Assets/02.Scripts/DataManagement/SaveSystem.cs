using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "savefile.json");

    public static void Save(GameData gameData)
    {
        string json = CustomJsonUtility.ToJson(gameData, true);
        Debug.Log("Saving JSON: " + json); // 저장되는 JSON 출력
        File.WriteAllText(savePath, json);
    }

    public static GameData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            Debug.Log("Loaded JSON: " + json);  // 디버깅을 위해 JSON 문자열 출력
            return CustomJsonUtility.FromJson<GameData>(json);
        }
        return null; // 새로운 데이터를 리턴
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}
