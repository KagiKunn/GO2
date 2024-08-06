using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class HeroDataWrapper
{
    public List<HeroData> Heroes = new List<HeroData>();
}

public class HeroGameManager : MonoBehaviour
{
    public List<HeroData> heroDataList; // ScriptableObject 목록
    public List<HeroData> selectedHeroes = new List<HeroData>(); // 영웅 편성 정보 저장(save)
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "selectedHeroes.json");
    }
    public void SaveHeroFormation()
    {
        HeroDataWrapper wrapper = new HeroDataWrapper { Heroes = selectedHeroes };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadHeroFormation()
    {   
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);

                selectedHeroes.Clear();

                foreach (HeroData hero in wrapper.Heroes)
                {
                    if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
                    {
                        selectedHeroes.Add(hero);
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
                selectedHeroes.Clear();
            }
        }
    }

    public void ClearHeroFormation()
    {
        selectedHeroes.Clear();
        SaveHeroFormation();
    }
    public void AddSelectedHero(HeroData hero)
    {
        if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
        {
            selectedHeroes.Add(hero);
        }
    }

    public void RemoveSelectedHero(HeroData hero)
    {
        selectedHeroes.Remove(hero);
    }

    public List<HeroData> GetHeroes()
    {
        return heroDataList;
    }

    public List<HeroData> GetSelectedHeroes()
    {
        return selectedHeroes;
    }
}
