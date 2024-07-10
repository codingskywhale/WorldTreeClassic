using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtility : MonoBehaviour
{
    public static string ToJson<T>(T obj, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}
