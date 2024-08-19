using System;

[Serializable]
public class PlayerLocalData
{
    //Local Save Data

    //Money for unit unlock
    public int Money { get; set; }
    public int RemainedPoint { get; set; }
    public int StartGold { get; set; }
    public int MoreEarnGold { get; set; }
    public int MoreCastleHealth { get; set; }
    public int ReduceCooldown { get; set; }
    public HeroList HerosList { get; set; }

    public PlayerLocalData()
    {
        Money = 0; // 초기 값 설정
        RemainedPoint = 0;
        StartGold = 0;
        MoreEarnGold = 0;
        MoreCastleHealth = 0;
        ReduceCooldown = 0;
        HerosList = new HeroList(null, false, false); // 초기 HeroList 생성
    }

    public PlayerLocalData(int money, int remainedPoint, int startGold, int moreEarnGold, int moreCastleHealth,
        int reduceCooldown, HeroList herosList)
    {
        Money = money;
        RemainedPoint = remainedPoint;
        StartGold = startGold;
        MoreEarnGold = moreEarnGold;
        MoreCastleHealth = moreCastleHealth;
        ReduceCooldown = reduceCooldown;
        HerosList = herosList;
    }
}

[Serializable]
public class HeroList : Triple<HeroDataSerializable, bool, bool>
{
    public HeroList(HeroDataSerializable hero, bool unlocked, bool selected) : base(hero, unlocked, selected)
    {
    }
}

[Serializable]
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

[Serializable]
public class HeroDataSerializable
{
    public string Name;
    public HeroDataSerializable(HeroData heroData)
    {
        Name = heroData.Name;
    }
}