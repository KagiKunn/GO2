using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class UnitGameManagerLeft : MonoBehaviour
{
    public List<UnitData> unitDataList;
    public List<UnitData> selectedUnits;
    private string filePath;

    public PlacementUnitLeft placementUnit;
    private static UnitGameManagerLeft instance;
    
    public static UnitGameManagerLeft Instance {
        get {
            if (instance == null) {
                instance = FindFirstObjectByType<UnitGameManagerLeft>();
                if (instance == null) {
                    GameObject go = new GameObject("UnitGameManagerLeft");
                    instance = go.AddComponent<UnitGameManagerLeft>();
                    DontDestroyOnLoad(go);
                    instance.InitializeFilePath();
                } else {
                    instance.InitializeFilePath();
                }
            }
            return instance;
        }
    }
    
    private void Awake() {
        Debug.Log("UnitGameManagerLeft Awake called");
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            instance.InitializeFilePath();
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }
    private void InitializeFilePath() {
        string savePath = Path.Combine(Application.dataPath, "save", "unitInfoLeft");
        Directory.CreateDirectory(savePath);
        filePath = Path.Combine(savePath, "selectedUnitsLeft.json");
    }
    
	public void SaveUnitFormation()
	{
		placementUnit.SavePlacementUnits();
		List<SlotUnitData> slotUnitDataList = placementUnit.GetSlotUnitDataList();

		SlotUnitDataWrapper wrapper = new SlotUnitDataWrapper(slotUnitDataList);
		string json = JsonUtility.ToJson(wrapper, true);
		File.WriteAllText(filePath, json);
		CustomLogger.Log("Unit formation saved successfully.");
	}

	// 유닛 편성 정보 불러오기
	public void LoadUnitFormation() {
		if (File.Exists(filePath)) {
			try {
				string json = File.ReadAllText(filePath);
				SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json); // SlotUnitDataWrapper로 변경
				selectedUnits.Clear();
				// 기존의 슬롯 배치를 초기화한 후, 저장된 데이터로 재배치
				if (wrapper != null && wrapper.SlotUnitDataList != null) {
					// 기존의 슬롯 배치를 초기화한 후, 저장된 데이터로 재배치
					placementUnit.SetSlotUnitDataList(wrapper.SlotUnitDataList);  // slotUnitDataList 업데이트

					// 필요한 경우 selectedUnits 리스트에 데이터 추가
					foreach (SlotUnitData slotUnit in wrapper.SlotUnitDataList) {
						if (selectedUnits.Count < 14) {
							selectedUnits.Add(slotUnit.UnitData);
						}
					}
					Debug.Log($"Loaded {wrapper.SlotUnitDataList.Count} slot unit data entries.");
				} else {
					Debug.LogWarning("SlotUnitDataWrapper or SlotUnitDataList is null.");
				}
			} catch (Exception e) {
				CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
				selectedUnits.Clear();
			}
		}
	}

	// reset시 유닛 편성 정보 제거(데이터 삭제)
	public void ClearUnitFormation() {
		instance.selectedUnits.Clear();
		SaveUnitFormation();
		CustomLogger.Log("리셋 성공");
		
	}
	
	// 유닛 드래그 해서 드롭 슬롯에 추가
	public void AddSelectedUnit(UnitData unit) {
		if (instance.selectedUnits.Count < 14) {
			instance.selectedUnits.Add(unit);
		}
	}

	// GetSet
	public List<UnitData> GetUnits() {
		return instance.unitDataList;
	}

	public List<UnitData> GetSelectedUnits() {
		return instance.selectedUnits;
	}

	// public void SetUpgradeUnit(UnitData unit) {
	// 	upgradeUnit = unit;
	// 	CustomLogger.Log("UpgradeUnit is" + unit.unitNumber);
	// }
	//
	// public UnitData GetUpgradeUnit() {
	// 	CustomLogger.Log("GetUpgradeHero called: " + (Instance.upgradeUnit != null ? Instance.upgradeUnit.unitNumber : "null"));
	//
	// 	return upgradeUnit;
	// }
	//
	// public void ClearUpgradeUnit() {
	// 	upgradeUnit = null;
	// }
}
