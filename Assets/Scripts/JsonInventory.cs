using System.IO;
using UnityEngine;

public static class JsonInventory
{
    public static void SaveToJson<T>(T data, string filePath)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static T LoadFromJson<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            CustomLogger.LogError($"File not found at {filePath}");
            return default(T);    
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<T>(json);

    }

}
