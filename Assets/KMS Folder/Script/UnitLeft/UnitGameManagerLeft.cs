using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class UnitGameManagerLeft : MonoBehaviour
{
    public List<UnitData> unitDataList;
    public Dictionary<UnitData, bool> unitPlacementStatus;
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
                } 
            }
            return instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
            instance.InitializeFilePath();
            
            if (unitDataList == null) {
                LoadUnitData();
                CustomLogger.Log(" unitDataList가 null인지 확인하고, 초기화");
            }
            if (unitPlacementStatus == null) {
                unitPlacementStatus = new Dictionary<UnitData, bool>();
                InitializeUnitPlacementStatus();
                CustomLogger.Log("unitPlacementStatus가 null인지 확인하고, 초기화");
            }
            if (placementUnit == null) {
                placementUnit = FindFirstObjectByType<PlacementUnitLeft>();
                CustomLogger.Log(" placementUnit가 null인지 확인하고, 초기화");
            }
            
            DontDestroyOnLoad(gameObject);
        } 
        else if (instance != this) {
            Destroy(instance.gameObject);
            instance = this;
            instance.InitializeFilePath();
            DontDestroyOnLoad(gameObject);
            LoadUnitData();
            InitializeUnitPlacementStatus();
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
        string savePath = Path.Combine(Application.dataPath, "save", "unitInfoLeft");
        Directory.CreateDirectory(savePath);
        filePath = Path.Combine(savePath, "selectedUnitsLeft.json");
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
                    Debug.Log($"Loaded {wrapper.SlotUnitDataList.Count} slot unit data entries.");
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
        string otherWallFilePath = Path.Combine(Application.dataPath, "save", "selectedUnits.json"); // 다른 성벽의 JSON 파일 경로
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
        if (unitDataList == null) {
            Debug.LogError("unitDataList is null in GetUnits.");
        }
        return instance.unitDataList;
    }
}