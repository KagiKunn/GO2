using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
// json 직렬화, 역직렬화 할때 중간 다리 역할을 하는 클래스
public class HeroManager : MonoBehaviour
{
    public List<HeroData> heroDataList; // ScriptableObject 목록
    public List<HeroData> selectedHeroes = new List<HeroData>();

    public HeroData upgradeHero; // 강화할 영웅 정보

    private static HeroManager instance;
    public static HeroManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<HeroManager>();

                // 씬에 HeroGameManger가 없는 경우
                if (instance == null)
                {
                    GameObject go = new GameObject("HeroGameManager");
                    instance = go.AddComponent<HeroManager>();
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
            LoadHeroFormation();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 영웅 편성 정보 저장(Save Btn)
    public void SaveHeroFormation()
    {
        HeroList[] heroList = new HeroList[heroDataList.Count];
        for (int i = 0; i < heroDataList.Count; i++)
        {
            HeroData hero = heroDataList[i];
            heroList[i] = new HeroList(hero.Name, true, 0, heroList[i].Item4);
        }

        foreach (HeroList heroL in heroList)
        {
            // selectedHeroes에서 heroL.Item1과 같은 이름을 가진 HeroData를 찾음
            HeroData selectedHero = Instance.selectedHeroes.Find(h => h.Name == heroL.Item1);

            if (selectedHero != null)
            {
                // selectedHero의 인덱스 번호를 찾아 heroL.Item3에 할당
                heroL.Item3 = Instance.selectedHeroes.IndexOf(selectedHero) + 1; // 1부터 시작하도록 설정
            }
        }

        PlayerLocalManager.Instance.lHeroeList = heroList;
        PlayerLocalManager.Instance.Save();
    }


    // 영웅 편성 정보 불러오기
    // 영웅 데이터 불러와서 쓰고 싶을땐 HeroSelect.cs의 LoadHeroFormation()을
    // 참고하시면 됩니다.
    public void LoadHeroFormation()
    {
        try
        {
            Instance.selectedHeroes.Clear();
            HeroList[] heroList = PlayerLocalManager.Instance.lHeroeList;

            foreach (HeroList hero in heroList)
            {
                // hero.Item1이 heroDataList에서 존재하는지 확인
                bool nameExists = heroDataList.Any(h => h.Name == hero.Item1);

                if (nameExists && hero.Item3 > 0)
                {
                    // selectedHeroes에 아직 추가되지 않았고, 최대 3개까지만 추가
                    if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Item1))
                    {
                        // hero.Item1과 일치하는 HeroData를 selectedHeroes에 추가
                        Instance.selectedHeroes.Add(heroDataList.Find(h => h.Name == hero.Item1));
                    }
                }
            }
            Instance.selectedHeroes = Instance.selectedHeroes.OrderBy(h => heroList.First(hero => hero.Item1 == h.Name).Item3).ToList();
        }
        catch (Exception e)
        {
            CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
            selectedHeroes.Clear();
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