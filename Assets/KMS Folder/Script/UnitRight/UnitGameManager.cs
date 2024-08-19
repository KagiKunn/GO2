using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class UnitDataWrapper {
    public List<UnitData> Units = new List<UnitData>();
}
public class UnitGameManager : MonoBehaviour {
    public List<UnitData> unitDataList; // ScriptableObject 목록
    public List<UnitData> selectedUnits; // 편성되어있는 유닛 정보
    private string filePath;

    public PlacementUnit placementUnit;
    private static UnitGameManager instance;

    public static UnitGameManager Instance {
        get {
            if (instance == null) {
                instance = FindFirstObjectByType<UnitGameManager>();
                if (instance == null) {
                    GameObject go = new GameObject("UnitGameManager");
                    instance = go.AddComponent<UnitGameManager>();
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
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            instance.InitializeFilePath();
			
            if (unitDataList == null) {
                unitDataList = new List<UnitData>();
                LoadUnitData(); // 씬 변환시에도 데이터를 로드하여 초기화
            }
            if (placementUnit == null) {
                placementUnit = FindFirstObjectByType<PlacementUnit>();
            }
			
        } 
        else if (instance != this) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFilePath();
        }
    }
	
    private void Start() {
        LoadUnitData();
    }
	
    private void LoadUnitData() {
        // 파일이나 다른 저장소에서 유닛 데이터를 로드하는 로직
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            unitDataList = JsonUtility.FromJson<List<UnitData>>(json);
        
            if (unitDataList == null) {
                unitDataList = new List<UnitData>();
                Debug.LogWarning("Loaded unit data is null, initializing an empty list.");
            }
        } else {
            Debug.LogWarning("No unit data file found, initializing an empty list.");
            unitDataList = new List<UnitData>();
        }
    }

    private void InitializeFilePath() {
        string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
        Directory.CreateDirectory(savePath);
        filePath = Path.Combine(savePath, "selectedUnits.json");
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
				
                if (wrapper != null && wrapper.SlotUnitDataList != null)
                {
                    placementUnit.SetSlotUnitDataList(wrapper.SlotUnitDataList);  // slotUnitDataList 업데이트

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