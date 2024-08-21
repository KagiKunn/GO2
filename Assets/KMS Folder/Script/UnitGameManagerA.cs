using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitGameManagerA : MonoBehaviour {
	public List<UnitData> unitDataList;
	public Dictionary<UnitData, bool> unitPlacementStatus;
	public string leftWallFilePath;
	public string rightWallFilePath;

	public PlacementUnitA leftPlacementUnit;
	public PlacementUnitA rightPlacementUnit;

	private static UnitGameManagerA instance;

	public static UnitGameManagerA Instance {
		get {
			if (instance == null) {
				instance = FindFirstObjectByType<UnitGameManagerA>();

				if (instance == null) {
					GameObject go = new GameObject("UnitGameManagerA");
					instance = go.AddComponent<UnitGameManagerA>();
					DontDestroyOnLoad(go);
					instance.InitializeFilePaths();
				} else {
					instance.InitializeFilePaths();
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

			InitializeFilePaths();

			if (unitPlacementStatus == null) {
				InitializeUnitPlacementStatus();
			}

			if (SceneManager.GetActiveScene().name == "UnitManagementLeft") {
				leftPlacementUnit = GameObject.Find("UnitDrop")?.GetComponent<PlacementUnitA>();
				LoadUnitFormation(leftPlacementUnit, leftWallFilePath);
				LoadUnitFormation(null, rightWallFilePath);
			} else if (SceneManager.GetActiveScene().name == "UnitManagementRight") {
				rightPlacementUnit = GameObject.Find("UnitDrop")?.GetComponent<PlacementUnitA>();
				LoadUnitFormation(rightPlacementUnit, rightWallFilePath);
				LoadUnitFormation(null, leftWallFilePath);
			}
		} else {
			Destroy(gameObject);
		}
	}
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("InternalAffairs");
        }
    }
    
	private void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == "UnitManagementLeft") {
			leftPlacementUnit = GameObject.Find("UnitDrop").GetComponent<PlacementUnitA>();
		} else if (scene.name == "UnitManagementRight") {
			rightPlacementUnit = GameObject.Find("UnitDrop").GetComponent<PlacementUnitA>();
		}
	}

	private void InitializeFilePaths() {
		string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");
		Directory.CreateDirectory(savePath);
		leftWallFilePath = Path.Combine(savePath, "selectedUnitsLeft.json");
		rightWallFilePath = Path.Combine(savePath, "selectedUnitsRight.json");
	}

	private void InitializeUnitPlacementStatus() {
		if (unitPlacementStatus == null) {
			Debug.LogWarning("placement status is null!");
			unitPlacementStatus = new Dictionary<UnitData, bool>();
		}
	}

	private void UpdatePlacementStatus(List<SlotUnitData> slotUnitDataList, bool isLeftWall) {
		if (slotUnitDataList == null) {
			Debug.LogError("slotUnitDataList is null in UpdatePlacementStatus!");

			return;
		}

		foreach (var slotUnit in slotUnitDataList) {
			if (slotUnit != null && slotUnit.UnitData != null) {
				unitPlacementStatus[slotUnit.UnitData] = true;
			} else {
				Debug.LogWarning("slotUnit or slotUnit.UnitData is null.");
			}
		}

		if (isLeftWall) {
			LoadAndTrackRightWallStatus();
		} else {
			LoadAndTrackLeftWallStatus();
		}
	}

	private void LoadAndTrackRightWallStatus() {
		if (File.Exists(rightWallFilePath)) {
			string json = File.ReadAllText(rightWallFilePath);
			SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);

			if (wrapper != null && wrapper.SlotUnitDataList != null) {
				foreach (var slotUnit in wrapper.SlotUnitDataList) {
					if (slotUnit.UnitData != null) {
						unitPlacementStatus[slotUnit.UnitData] = true;
					}
				}
			}
		}
	}

	private void LoadAndTrackLeftWallStatus() {
		if (File.Exists(leftWallFilePath)) {
			string json = File.ReadAllText(leftWallFilePath);
			SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);

			if (wrapper != null && wrapper.SlotUnitDataList != null) {
				foreach (var slotUnit in wrapper.SlotUnitDataList) {
					if (slotUnit.UnitData != null) {
						unitPlacementStatus[slotUnit.UnitData] = true;
					}
				}
			}
		}
	}

	private void LoadUnitData() {
		unitDataList = new List<UnitData>(Resources.LoadAll<UnitData>("ScriptableObjects/Unit"));
	}

	public UnitData LoadUnitDataById(int id) {
		UnitData[] allUnits = Resources.LoadAll<UnitData>("ScriptableObjects/Unit");

		foreach (var unit in allUnits) {
			if (unit.ID == id) {
				return unit;
			}
		}

		return null;
	}

	public void SaveUnitFormation(PlacementUnitA placementUnit, string filePath) {
		placementUnit.SavePlacementUnits();
		List<SlotUnitData> slotUnitDataList = placementUnit.GetSlotUnitDataList();

		foreach (var unit in unitPlacementStatus.Keys.ToList()) {
			unitPlacementStatus[unit] = false;
		}

		foreach (var slotUnit in slotUnitDataList) {
			if (slotUnit.UnitData != null) {
				unitPlacementStatus[slotUnit.UnitData] = true;
			}
		}

		SlotUnitDataWrapper wrapper = new SlotUnitDataWrapper(slotUnitDataList);
		string json = JsonUtility.ToJson(wrapper, true);
		File.WriteAllText(filePath, json);
	}

	public void LoadUnitFormation(PlacementUnitA placementUnit, string filePath) {
		if (File.Exists(filePath)) {
			try {
				string json = File.ReadAllText(filePath);
				SlotUnitDataWrapper wrapper = JsonUtility.FromJson<SlotUnitDataWrapper>(json);

				if (wrapper != null && wrapper.SlotUnitDataList != null) {
					foreach (var slotUnit in wrapper.SlotUnitDataList) {
						if (slotUnit.UnitData != null) {
							slotUnit.UnitData = LoadUnitDataById(slotUnit.UnitData.ID);
						}
					}

					if (placementUnit != null) {
						placementUnit.SetSlotUnitDataList(wrapper.SlotUnitDataList);
					}

					UpdatePlacementStatus(wrapper.SlotUnitDataList, placementUnit == leftPlacementUnit);
				} else {
					Debug.LogWarning("SlotUnitDataWrapper or SlotUnitDataList is null.");
				}
			} catch (Exception e) {
				Debug.LogError($"Error loading unit formation: {e.Message}");
			}
		} else {
			Debug.LogError("File does not exist: " + filePath);
			// json 기본 생성 해주기
			List<SlotUnitData> defaultData = new List<SlotUnitData>();
			SlotUnitDataWrapper wrapper = new SlotUnitDataWrapper(defaultData);
			string json = JsonUtility.ToJson(wrapper, true);
			File.WriteAllText(filePath, json);

			if (placementUnit != null) {
				placementUnit.SetSlotUnitDataList(defaultData);
			}
		}
	}

	public void ClearUnitFormation(PlacementUnitA placementUnit, string filePath) {
		var slotUnitDataList = placementUnit.GetSlotUnitDataList();

		foreach (var slotUnit in slotUnitDataList) {
			unitPlacementStatus[slotUnit.UnitData] = false;
		}

		placementUnit.SetSlotUnitDataList(new List<SlotUnitData>());
		SaveUnitFormation(placementUnit, filePath);
	}

	public bool IsUnitSelected(UnitData unit) {
		return unitPlacementStatus.ContainsKey(unit) && unitPlacementStatus[unit];
	}

	public List<UnitData> GetUnits() {
		return unitDataList;
	}

	public string GetFilePath(PlacementUnitA placementUnit) {
		if (placementUnit == leftPlacementUnit) {
			return leftWallFilePath;
		} else if (placementUnit == rightPlacementUnit) {
			return rightWallFilePath;
		}

		return null;
	}
}