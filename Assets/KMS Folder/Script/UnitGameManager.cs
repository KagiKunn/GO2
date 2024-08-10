using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using UnityEngine;

[System.Serializable]
public class UnitDataWrapper
{
    public List<UnitData> Units = new List<UnitData>();
}

public class UnitGameManager : MonoBehaviour
{
    public List<UnitData> unitDataList; // ScriptableObject 목록
    public List<UnitData> selectedUnits; // 편성되어있는 유닛 정보
    private string filePath;

    public UnitData upgradeUnit; // 강화할 유닛 정보

    private static UnitGameManager instance;
    public static UnitGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UnitGameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("UnitGameManager");
                    instance = go.AddComponent<UnitGameManager>();
                    DontDestroyOnLoad(go);
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
            Directory.CreateDirectory(savePath);
            filePath = Path.Combine(savePath, "selectedUnits.json");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // 유닛 편성 정보 저장(Save Btn)
    public void SaveUnitFormation()
    {
        UnitDataWrapper wrapper = new UnitDataWrapper { Units = selectedUnits };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }
    // 유닛 편성 정보 불러오기
    public void LodeUnitFormation()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                UnitDataWrapper wrapper = JsonUtility.FromJson<UnitDataWrapper>(json);
                
                Instance.selectedUnits.Clear();
                foreach (UnitData unit in wrapper.Units)
                {
                    // 14명으로 편성 제한
                    if (selectedUnits.Count < 14 )
                    {
                        Instance.selectedUnits.Add(unit);
                    }
                }
                
            }
            catch (Exception e)
            {
                CustomLogger.Log($"Error loading hero formation: {e.Message}", "red");
                selectedUnits.Clear();
            }
        }
    }
    
    // reset시 유닛 편성 정보 제거(데이터 삭제)
    public void ClearUnitFormation()
    {
        Instance.selectedUnits.Clear();
        SaveUnitFormation();
    }
    
    // 유닛 드래그 해서 드롭 슬롯에 추가
    public void AddSelectedUnit(UnitData unit)
    {
        if (selectedUnits.Count < 14)
        {
            Instance.selectedUnits.Add(unit);
        }
    }

    // GetSet
    public List<UnitData> GetUnits()
    {
        return Instance.unitDataList;
    }

    public List<UnitData> GetSelectedUnits()
    {
        return Instance.selectedUnits;
    }
    
    public void SetUpgradeUnit(UnitData unit)
    {
        Instance.upgradeUnit = unit;
        CustomLogger.Log("UpgradeUnit is" + unit.unitNumber);
    }

    public UnitData GetUpgradeUnit()
    {
        CustomLogger.Log("GetUpgradeHero called: " + (Instance.upgradeUnit != null ? Instance.upgradeUnit.unitNumber : "null"));
        return Instance.upgradeUnit;
    }

    public void ClearUpgradeUnit()
    {
        Instance.upgradeUnit = null;
    }
}
