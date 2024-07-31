using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroGameManager : MonoBehaviour
{
    public LocalDataManager localDataManager;
    public List<HeroData> heroDataList; // ScriptableObject 목록

    private List<HeroData> heroes = new List<HeroData>();
    void Start()
    {
        heroes = localDataManager.LoadHeroData();
    }

    private void OnApplicationQuit()
    {
        localDataManager.SaveHeroData(heroDataList);
    }

    public List<HeroData> GetHeroes()
    {
        return heroDataList;
    }
    
}
