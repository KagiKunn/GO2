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
    
    public HeroData upgradeHero;// 강화할 영웅 정보

    private static HeroGameManager instance;
    
    public static HeroGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<HeroGameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("HeroGameManager");
                    instance = go.AddComponent<HeroGameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "selectedHeroes.json");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
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

    public void SetUpgradeHero(HeroData hero)
    {
        upgradeHero = hero;
        Debug.Log("SetUpgradeHero called with: " + hero.Name);
    }

    public HeroData GetUpgradeHero()
    {
        Debug.Log("GetUpgradeHero called: " + (upgradeHero != null ? upgradeHero.Name : "null"));
        return upgradeHero;
    }
    
    public void ClearUpgradeHero()
    {
        upgradeHero = null;
    }
}
