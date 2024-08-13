using UnityEngine;

[System.Serializable]
public class Bounus
{
    public bool startGold;
    public bool moreEarnGold;
    public bool moreCastleHealth;
    public bool reduceCooldown;

    public bool startGold1
    {
        get => startGold;
        set => startGold = value;
    }

    public bool moreEarnGold1
    {
        get => moreEarnGold;
        set => moreEarnGold = value;
    }

    public bool moreCastleHealth1
    {
        get => moreCastleHealth;
        set => moreCastleHealth = value;
    }

    public bool reduceCooldown1
    {
        get => reduceCooldown;
        set => reduceCooldown = value;
    }
}

[CreateAssetMenu(fileName = "BounusData", menuName = "ScriptableObjects/BounusData")]
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