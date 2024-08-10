using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
// json 직렬화, 역직렬화 할때 중간 다리 역할을 하는 클래스
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
    // 영웅 편성 정보를 영속적으로(씬 전환, 게임 종료 후에도 상관없이) 저장하기 위해 직렬화함
    // Instance 안할 시 영웅 데이터가 일률적으로 관리가 되지 않아 오류 발생
    public static HeroGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<HeroGameManager>();
                // 씬에 HeroGameManger가 없는 경우
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
    
    // 영웅 편성 정보 저장(Save Btn)
    public void SaveHeroFormation()
    {
        HeroDataWrapper wrapper = new HeroDataWrapper { Heroes = selectedHeroes };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }
    // 영웅 편성 정보 불러오기
    // 영웅 데이터 불러와서 쓰고 싶을땐 HeroSelect.cs의 LoadHeroFormation()을
    // 참고하시면 됩니다. 
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
                    // 동일 영웅 중복 저장 방지 및 3명으로 제한
                    if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
                    {
                        Instance.selectedHeroes.Add(hero);
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
    // resetBtn눌렀을때 비우기
    public void ClearHeroFormation()
    {
        Instance.selectedHeroes.Clear();
        SaveHeroFormation();
    }
    
    // 영웅 슬롯에 추가
    public void AddSelectedHero(HeroData hero)
    {
        if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name))
        {
            Instance.selectedHeroes.Add(hero);
        }
    }
    
    // GetSet
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
