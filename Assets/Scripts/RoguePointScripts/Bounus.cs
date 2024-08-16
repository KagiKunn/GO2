using UnityEngine;

[System.Serializable]
public class Bounus
{
    public int startGold;
    public int moreEarnGold;
    public int moreCastleHealth;
    public int reduceCooldown;
    
    public int startGold1
    {
        get => startGold;
        set => startGold = value;
    }

    public int moreEarnGold1
    {
        get => moreEarnGold;
        set => moreEarnGold = value;
    }

    public int moreCastleHealth1
    {
        get => moreCastleHealth;
        set => moreCastleHealth = value;
    }

    public int reduceCooldown1
    {
        get => reduceCooldown;
        set => reduceCooldown = value;
    }
}

[CreateAssetMenu(fileName = "Bounus", menuName = "ScriptableObjects/BounusData")]
public class BounusScriptableObject : ScriptableObject
{
    public Bounus data;

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
            data = JsonUtility.FromJson<Bounus>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found in " + filePath);
        }
    }
}