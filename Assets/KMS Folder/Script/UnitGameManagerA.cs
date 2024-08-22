using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitGameManagerA : MonoBehaviour
{
    public List<UnitData> unitDataList;
    public Dictionary<UnitData, int> unitPlacementStatus; // 0: 미배치, 1: 왼쪽 성벽, 2: 오른쪽 성벽
    private List<SlotUnitData> slotUnitDataList = new List<SlotUnitData>();
    private static UnitGameManagerA instance;
    public static UnitGameManagerA Instance {
        get {
            if (instance == null) {
                instance = FindFirstObjectByType<UnitGameManagerA>();
                if (instance == null) {
                    GameObject go = new GameObject("UnitGameManagerA");
                    instance = go.AddComponent<UnitGameManagerA>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    void Awake() {
        if (instance == null) 
        {
            instance = this; 
            DontDestroyOnLoad(gameObject);

            if (unitDataList == null || unitDataList.Count == 0) {
                LoadUnitData();
            }

            if (unitPlacementStatus == null) {
                InitializeUnitPlacementStatus();
            }
            
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeUnitPlacementStatus()
    {
            unitPlacementStatus = new Dictionary<UnitData, int>();
            foreach (var unit in unitDataList) {
                unitPlacementStatus[unit] = 0;
            }
    }

    private void LoadUnitData() 
    {
        unitDataList = new List<UnitData>(Resources.LoadAll<UnitData>("ScriptableObjects/Unit"));
    }

    public void SaveUnitPlacement(int slotIndex, UnitData unit, int placement)
    {
        instance.unitPlacementStatus[unit] = placement;
        CustomLogger.Log(placement);
        
        if (slotIndex < 0)
        {
            return;
        }
        
        if (instance.slotUnitDataList.Count <= slotIndex)
        {
            for (int i = instance.slotUnitDataList.Count; i <= slotIndex; i++)
            {
                instance.slotUnitDataList.Add(null);
            }
        }
        
        var slotUnitData = new SlotUnitData(slotIndex, unit, placement);
        instance.slotUnitDataList[slotIndex] = slotUnitData;
        
    }

    public List<UnitData> GetUnits() {
        return unitDataList;
    }
    
    public void SaveSlotUnitData(List<SlotUnitData> dataList)
    {
        List<SlotUnitData> existingDataList = LoadSlotUnitData();
        
        foreach (var newData in dataList)
        {
            int index = existingDataList.FindIndex(d => d.SlotIndex == newData.SlotIndex);
            if (index >= 0)
            {
                existingDataList[index] = newData;
            }
            else
            {
                existingDataList.Add(newData);
            }
        }
        
        if (dataList.Count == 0)
        {
            Debug.LogWarning("No data to save.");
            return;
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            CustomLogger.Log("Data saved to: " + dataList[i], Color.magenta);
        }
        
        string json = JsonUtility.ToJson(new SlotUnitDataWrapper(dataList), true);

        string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
        Directory.CreateDirectory(savePath);
        string filePath = Path.Combine(savePath, "selectedUnits.json");

        File.WriteAllText(filePath, json);

        CustomLogger.Log("Data saved to: " + filePath, Color.magenta);
    }

    
    public List<SlotUnitData> LoadSlotUnitData()
    {
        string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
        string filePath = Path.Combine(savePath, "selectedUnits.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);
            return wrapper.SlotUnitDataList;

        }
        else
        {
            Debug.LogWarning("저장된 유닛 데이터 파일을 찾을 수 없습니다.");
            return new List<SlotUnitData>();
        }
        
    }
    
}
