using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerId;
    public int money;
    public int soul;

    public string playerId1
    {
        get => playerId;
        set => playerId = value;
    }

    public int money1
    {
        get => money;
        set => money = value;
    }

    public int soul1
    {
        get => soul;
        set => soul = value;
    }
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerDataScriptableObject : ScriptableObject
{
    public PlayerData data;

    public void SaveToJson(string filePath)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(filePath, jsonData);
    }

    public void LoadFromJson(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            string jsonData = System.IO.File.ReadAllText(filePath);
            data = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found in " + filePath);
        }
    }
}