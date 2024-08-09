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

            string savePath = Path.Combine(Application.dataPath, "save", "heroInfo");
            Directory.CreateDirectory(savePath);
            filePath = Path.Combine(savePath, "selectedHeroes.json");
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

                Instance.selectedHeroes.Clear();

                foreach (HeroData hero in wrapper.Heroes)
                {
                    if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
                    {
                        Instance.selectedHeroes.Add(hero);
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
                CustomLogger.Log($"StackTrace: {e.StackTrace}", "red");
                selectedHeroes.Clear();
            }
        }
    }

    public void ClearHeroFormation()
    {
        Instance.selectedHeroes.Clear();
        SaveHeroFormation();
    }
    
    public void AddSelectedHero(HeroData hero)
    {
        if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
        {
            Instance.selectedHeroes.Add(hero);
        }
    }

    public void RemoveSelectedHero(HeroData hero)
    {
        Instance.selectedHeroes.Remove(hero);
    }

    public List<HeroData> GetHeroes()
    {
        return Instance.heroDataList;
    }

    public List<HeroData> GetSelectedHeroes()
    {
        return Instance.selectedHeroes;
    }

    public void SetUpgradeHero(HeroData hero)
    {
        Instance.upgradeHero = hero; 
        Debug.Log("SetUpgradeHero called with: " + hero.Name);
    }

    public HeroData GetUpgradeHero()
    {
        Debug.Log("GetUpgradeHero called: " + (Instance.upgradeHero != null ? Instance.upgradeHero.Name : "null"));
        return Instance.upgradeHero;
    }
    
    public void ClearUpgradeHero()
    {
        Instance.upgradeHero = null;
    }
}
