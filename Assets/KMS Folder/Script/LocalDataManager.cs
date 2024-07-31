using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
// 직렬화
public class HeroDataWrapper
{
    public List<HeroData> Heroes = new List<HeroData>();
}

public class LocalDataManager : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "heroes.json");
    }

    public void SaveHeroData(List<HeroData> heroes)
    {
        HeroDataWrapper wrapper = new HeroDataWrapper { Heroes = heroes };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public List<HeroData> LoadHeroData()
    {
        try
        {
            string json = File.ReadAllText(filePath);
            HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);
            return wrapper.Heroes;
        }
        catch (Exception e)
        {
            return new List<HeroData>();
            CustomLogger.Log("There's error on Hero Data->" + e.Message);
        }
    }
}
