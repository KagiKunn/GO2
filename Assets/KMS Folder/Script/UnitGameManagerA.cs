using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitGameManagerA : MonoBehaviour
{
    public List<UnitData> unitDataList;
    public Dictionary<UnitData, int> unitPlacementStatus; // 0: 미배치, 1: 왼쪽 성벽, 2: 오른쪽 성벽

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
        if (instance == null) {
            
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

    public void SaveUnitPlacement(UnitData unit, int placement)
    {
        if (unitPlacementStatus.ContainsKey(unit)) {
            unitPlacementStatus[unit] = placement;
        }
    }

    public List<UnitData> GetUnits() {
        return unitDataList;
    }
}
