using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.Localization.Settings;

[Serializable]
public class PlayerLocalData {
	//Local Save Data

	//Money for unit unlock
	public int Money { get; set; }
	public int RemainedPoint { get; set; }
	public int StartGold { get; set; }
	public int MoreEarnGold { get; set; }
	public int MoreCastleHealth { get; set; }
	public int ReduceCooldown { get; set; }
	public HeroList[] HerosList { get; set; }

	public int Stage { get; set; }
	public string[] StageRace { get; set; }

	public string SelectedRace { get; set; }
	public float CastleMaxHealth { get; set; }
	public float CastleHealth { get; set; }
	public float CastleExtraHealth { get; set; }
	public bool GameStarted { get; set; }
	public bool NextEnemy { get; set; }

	public new List<Triple<int, int, string>> UnitList { get; set; }

	public string Locale { get; set; }

	public PlayerLocalData() {
		Money = 1000; // 초기 값 설정
		RemainedPoint = 0;
		StartGold = 0;
		MoreEarnGold = 0;
		MoreCastleHealth = 0;
		ReduceCooldown = 0;
		HerosList = new HeroList[1]; // 크기를 1로 지정 (원하는 크기로 변경 가능)
		HerosList[0] = new HeroList(null, false, 0, null); // 배열의 첫 번째 요소 초기화
		Stage = 0;
		StageRace = new string[] { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
		SelectedRace = null;
		CastleMaxHealth = 5000f;
		CastleHealth = 5000f;
		CastleExtraHealth = 0;
		UnitList = new List<Triple<int, int, string>>();

		for (int i = 0; i < 28; i++) {
			Triple<int, int, string> keyVals = new Triple<int, int, string>(i, 0, "Default");
			UnitList.Add(keyVals);
		}

		GameStarted = false;
		Locale = LocalizationSettings.SelectedLocale.Identifier.Code;
		NextEnemy = false;
	}

	public PlayerLocalData(int money, int remainedPoint, int startGold, int moreEarnGold, int moreCastleHealth,
	                       int reduceCooldown, HeroList[] herosList, int stage, string[] stageRace, string selectedRace, float castleMaxHealth,
	                       float castleHealth, float castleExtraHealth, List<Triple<int, int, string>> unitList, bool gameStarted,
	                       string locale, bool nextEnemy) {
		Money = money;
		RemainedPoint = remainedPoint;
		StartGold = startGold;
		MoreEarnGold = moreEarnGold;
		MoreCastleHealth = moreCastleHealth;
		ReduceCooldown = reduceCooldown;
		HerosList = herosList;
		Stage = stage;
		StageRace = stageRace;
		SelectedRace = selectedRace;
		CastleMaxHealth = castleMaxHealth;
		CastleHealth = castleHealth;
		CastleExtraHealth = castleExtraHealth;
		UnitList = unitList;
		GameStarted = gameStarted;
		Locale = locale;
		NextEnemy = nextEnemy;
	}
}

[Serializable]
public class HeroList : Quad<string, bool, int, ItemSO[]> {
	public HeroList(string heroName, bool unlocked, int selected, ItemSO[] equip) : base(heroName, unlocked, selected, equip) { }

	public HeroList(string heroName, bool unlocked, int selected) : base(heroName, unlocked, selected) { }
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
	public static Triple<T1, T2, T3> GetTripleWithMaxT1<T1, T2, T3>(List<Triple<T1, T2, T3>> triples) where T1 : IComparable<T1>
	{
		if (triples == null || triples.Count == 0)
		{
			throw new ArgumentException("The list cannot be null or empty.");
		}

		Triple<T1, T2, T3> maxTriple = triples[0];

		foreach (var triple in triples)
		{
			if (triple.Item1.CompareTo(maxTriple.Item1) > 0)
			{
				maxTriple = triple;
			}
		}

		return maxTriple;
	}
}


[Serializable]
public class Quad<T1, T2, T3, T4> {
	public T1 Item1 { get; set; }
	public T2 Item2 { get; set; }
	public T3 Item3 { get; set; }
	public T4 Item4 { get; set; }

	public Quad(T1 item1, T2 item2, T3 item3, T4 item4) {
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
		Item4 = item4;
	}

	public Quad(T1 item1, T2 item2, T3 item3) {
		Item1 = item1;
		Item2 = item2;
		Item3 = item3;
	}
}