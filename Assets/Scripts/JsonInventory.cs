using System;
using System.IO;
using UnityEngine;

public static class JsonInventory
{
    public static void SaveToJson<T>(T data, string filePath)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            CustomLogger.LogError($"Failed to write JSON to file: {ex.Message}");
        }
      
    }

    public static T LoadFromJson<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            CustomLogger.LogError($"File not found at {filePath}");
            return default(T);    
        }

        string json = File.ReadAllText(filePath);
        CustomLogger.Log($"JSON content: {json}");
        return JsonUtility.FromJson<T>(json);

    }

}