using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

[System.Serializable]
// json 직렬화, 역직렬화 할때 중간 다리 역할을 하는 클래스
public class HeroDataWrapper {
	public List<HeroData> Heroes = new List<HeroData>();
}

public class HeroGameManager : MonoBehaviour {
	public List<HeroData> heroDataList; // ScriptableObject 목록
	public List<HeroData> selectedHeroes = new List<HeroData>(); // 영웅 편성 정보 저장(save)
	public List<HeroData> unselectedHeroes = new List<HeroData>(); // 편성되지 않은 영웅 정보 저장
	private string filePath;

	public HeroData upgradeHero; // 강화할 영웅 정보

	private static HeroGameManager instance;
	// 영웅 편성 정보를 영속적으로(씬 전환, 게임 종료 후에도 상관없이) 저장하기 위해 직렬화함
	// Instance 안할 시 영웅 데이터가 일률적으로 관리가 되지 않아 오류 발생

	// 다른 스크립트에서 데이터를 활용할 시 먼저 Load메서드로 데이터를 로드 한 뒤(HeroGameManager.Instance.LoadUnselectedHeroes();)
	// 로드 된 데이터를 List로 가져오셔서 (Get 메서드) 활용하시면 됩니다. (List<HeroData> unselectedHeroes = HeroGameManager.Instance.GetUnselectedHeroes();)
	// 활용 예시
	//  if (unselectedHeroes.Count > 0)
	// {
	//     HeroData firstHero = unselectedHeroes[0];
	//     Debug.Log("First Unselected Hero Name: " + firstHero.Name);
	//     Debug.Log("First Unselected Hero Attack: " + firstHero.Attack);
	// }
	public static HeroGameManager Instance {
		get {
			if (instance == null) {
				instance = FindFirstObjectByType<HeroGameManager>();

				// 씬에 HeroGameManger가 없는 경우
				if (instance == null) {
					GameObject go = new GameObject("HeroGameManager");
					instance = go.AddComponent<HeroGameManager>();
					DontDestroyOnLoad(go);
				}
			}

			return instance;
		}
	}

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);

			string savePath = Path.Combine(Application.dataPath, "save", "heroInfo");
			Directory.CreateDirectory(savePath);
			filePath = Path.Combine(savePath, "selectedHeroes.json");
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	// 영웅 편성 정보 저장(Save Btn)
	public void SaveHeroFormation() {
		HeroDataWrapper wrapper = new HeroDataWrapper { Heroes = selectedHeroes };
		string json = JsonUtility.ToJson(wrapper, true);
		File.WriteAllText(filePath, json);
		SaveUnselectedHeroes(); // 편성되지 않은 영웅도 따로 저장
	}

	// 영웅 편성 정보 불러오기
	// 영웅 데이터 불러와서 쓰고 싶을땐 HeroSelect.cs의 LoadHeroFormation()을
	// 참고하시면 됩니다. 
	public void LoadHeroFormation() {
		if (File.Exists(filePath)) {
			try {
				string json = File.ReadAllText(filePath);
				HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);

				Instance.selectedHeroes.Clear();

				foreach (HeroData hero in wrapper.Heroes) {
					// 동일 영웅 중복 저장 방지 및 3명으로 제한
					if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name)) {
						Instance.selectedHeroes.Add(hero);
					}
				}
			} catch (Exception e) {
				CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
				selectedHeroes.Clear();
			}
		}
	}

	// 편성되지 않은 나머지 영웅들 정보 저장
	public void SaveUnselectedHeroes() {
		heroDataList = GetHeroes();
		selectedHeroes = GetSelectedHeroes();
		unselectedHeroes.Clear();

		foreach (var hero in heroDataList) {
			if (!selectedHeroes.Exists(h => h.Name == hero.Name)) {
				Instance.unselectedHeroes.Add(hero);
			}
		}

		HeroDataWrapper wrapper = new HeroDataWrapper { Heroes = unselectedHeroes };
		string json = JsonUtility.ToJson(wrapper, true);
		string unselectedFilePath = Path.Combine(Application.dataPath, "save", "heroInfo", "unselectedHeroes.json");
		File.WriteAllText(unselectedFilePath, json);
	}
	// 편성되지 않은 나머지 영웅들 정보 불러오기(unselectedHeroes.json)

	public void LoadUnselectedHeroes() {
		string unselectedFilePath = Path.Combine(Application.dataPath, "save", "heroInfo", "unselectedHeroes.json");

		if (File.Exists(unselectedFilePath)) {
			try {
				string json = File.ReadAllText(unselectedFilePath);
				HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);

				Instance.unselectedHeroes.Clear();
				Instance.unselectedHeroes.AddRange(wrapper.Heroes);
			} catch (Exception e) {
				CustomLogger.Log($"Error loading unselected heroes: {e.Message}", "red");
				unselectedHeroes.Clear();
			}
		}
	}

	// resetBtn눌렀을때 비우기
	public void ClearHeroFormation() {
		Instance.selectedHeroes.Clear();
		SaveHeroFormation();
	}

	// 영웅 슬롯에 추가
	public void AddSelectedHero(HeroData hero) {
		if (selectedHeroes.Count < 3 && !selectedHeroes.Exists(h => h.Name == hero.Name)) {
			Instance.selectedHeroes.Add(hero);
		}
	}

	// GetSet
	public List<HeroData> GetHeroes() {
		return Instance.heroDataList;
	}

	public List<HeroData> GetSelectedHeroes() {
		return Instance.selectedHeroes;
	}

	public List<HeroData> GetUnselectedHeroes() {
		return Instance.unselectedHeroes;
	}

	public void SetUpgradeHero(HeroData hero) {
		Instance.upgradeHero = hero;
		Debug.Log("SetUpgradeHero called with: " + hero.Name);
	}

	public HeroData GetUpgradeHero() {
		Debug.Log("GetUpgradeHero called: " + (Instance.upgradeHero != null ? Instance.upgradeHero.Name : "null"));

		return Instance.upgradeHero;
	}

	public void ClearUpgradeHero() {
		Instance.upgradeHero = null;
	}
}