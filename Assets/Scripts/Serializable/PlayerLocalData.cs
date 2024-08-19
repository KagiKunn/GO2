using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLocalData
{
    //Local Save Data
    
    //Money for unit unlock
    private int money;
    
    //Rogue Point
    private int startGold;
    private int moreEarnGold;
    private int moreCastleHealth;
    private int reduceCooldown;
    
    //Hero List
    private HeroList HeroeList;
}

public class HeroList : Triple<HeroData, bool, bool>
{
    public HeroList(HeroData Hero, bool Unlocked, bool Selected) : base(Hero, Unlocked, Selected)
    {
    }
}

public class Triple<T1, T2, T3>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public T3 Item3 { get; set; }

    public Triple(T1 item1, T2 item2, T3 item3)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }
}