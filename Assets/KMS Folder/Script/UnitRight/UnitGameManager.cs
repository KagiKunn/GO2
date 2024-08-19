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
    public List<UnitData> unitDataList;
    public Dictionary<UnitData, bool> unitPlacementStatus;
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
                }else {
                    instance.InitializeFilePath();
                }
            }
            return instance;
        }
    }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            instance.InitializeFilePath();

            // placementUnit이 null일 경우 초기화
            if (placementUnit == null) {
                GameObject unitDropObject = GameObject.Find("UnitDrop");
                if (unitDropObject != null) {
                    placementUnit = unitDropObject.GetComponent<PlacementUnit>();
                } else {
                    Debug.LogError("UnitDrop 오브젝트를 찾을 수 없습니다.");
                }
            }
            
            if (unitDataList == null)
            {
                LoadUnitData();
                CustomLogger.Log(" unitDataList가 null인지 확인하고, 초기화");
            }
            if (unitPlacementStatus == null)
            {
                unitPlacementStatus = new Dictionary<UnitData, bool>();
                InitializeUnitPlacementStatus();
                CustomLogger.Log("unitPlacementStatus가 null인지 확인하고, 초기화");
            }

            if (placementUnit == null)
            {
                GameObject unitDropObject = GameObject.Find("UnitDrop");
                placementUnit = unitDropObject.GetComponent<PlacementUnit>();
                CustomLogger.Log(" placementUnit가 null인지 확인하고, 초기화");
            }
            DontDestroyOnLoad(gameObject);
            LoadUnitFormation();
        }
        if (instance != this) {
            Destroy(instance.gameObject);  // 기존 인스턴스가 남아 있으면 삭제
            instance = this;
            instance.InitializeFilePath();
            DontDestroyOnLoad(gameObject);
            LoadUnitData();
            InitializeUnitPlacementStatus();
            LoadUnitFormation();
        }
    }

    private void InitializeUnitPlacementStatus() {
        if (unitPlacementStatus == null) {
            unitPlacementStatus = new Dictionary<UnitData, bool>();
        }

        foreach (var unit in unitDataList) {
            if (!unitPlacementStatus.ContainsKey(unit)) {
                unitPlacementStatus[unit] = false;
            }
        }
        
        List<SlotUnitData> slotUnitDataList = placementUnit.GetSlotUnitDataList();
        foreach (var slotUnit in slotUnitDataList) {
            if (unitPlacementStatus.ContainsKey(slotUnit.UnitData)) {
                unitPlacementStatus[slotUnit.UnitData] = true;
            }
        }
    }
    private void LoadUnitData() {
        unitDataList = new List<UnitData>(Resources.LoadAll<UnitData>("ScriptableObjects/Unit"));
    }

    private void InitializeFilePath() 
    {
        string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
        Directory.CreateDirectory(savePath);
        filePath = Path.Combine(savePath, "selectedUnits.json");
    }

    public void SaveUnitFormation()
    {
        if (string.IsNullOrEmpty(filePath)) {
            instance.InitializeFilePath();
        }
        
        placementUnit.SavePlacementUnits();
        List<SlotUnitData> slotUnitDataList = placementUnit.GetSlotUnitDataList();

        SlotUnitDataWrapper wrapper = new SlotUnitDataWrapper(slotUnitDataList);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
        
        CustomLogger.Log("Unit formation saved successfully.");
    }

    public void LoadUnitFormation() {
        if (File.Exists(filePath)) {
            try {
                string json = File.ReadAllText(filePath);
                SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);
                
                if (wrapper != null && wrapper.SlotUnitDataList != null)
                {
                    placementUnit.SetSlotUnitDataList(wrapper.SlotUnitDataList);
                } else {
                    Debug.LogWarning("SlotUnitDataWrapper or SlotUnitDataList is null.");
                }
            } catch (Exception e) {
                CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
            }
        }
    }

    
    public void ClearUnitFormation() {
        placementUnit.SetSlotUnitDataList(new List<SlotUnitData>());
        SaveUnitFormation();
        CustomLogger.Log("리셋 성공");
    }
    
    public bool IsUnitSelected(UnitData unit) {
        if (placementUnit == null) {
            Debug.LogWarning("placementUnit is null in IsUnitSelected!");
            return false;
        }
        // 현재 성벽에 배치된 유닛 정보를 확인
        List<SlotUnitData> slotUnitDataList = placementUnit.GetSlotUnitDataList();
        foreach (var slotUnit in slotUnitDataList) {
            if (slotUnit.UnitData == unit) {
                return true;
            }
        }

        // 다른 성벽에 배치된 유닛 정보를 확인 (JSON 파일 로드)
        string otherWallFilePath = Path.Combine(Application.dataPath, "unitInfoLeft", "selectedUnitsLeft.json"); // 다른 성벽의 JSON 파일 경로
        if (File.Exists(otherWallFilePath)) {
            string json = File.ReadAllText(otherWallFilePath);
            SlotUnitDataWrapper otherWallWrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);
            if (otherWallWrapper != null && otherWallWrapper.SlotUnitDataList != null) {
                foreach (var slotUnit in otherWallWrapper.SlotUnitDataList) {
                    if (slotUnit.UnitData == unit) {
                        return true; // 다른 성벽에 배치된 유닛이 있음
                    }
                }
            }
        }

        return false; // 어느 성벽에도 배치되지 않은 경우
    }
    
    public List<UnitData> GetUnits() {
        return instance.unitDataList;
    }
}