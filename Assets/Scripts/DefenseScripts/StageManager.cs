using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] public RacePrefabs darkElfPrefabs;
    [SerializeField] public RacePrefabs humanPrefabs;
    [SerializeField] public RacePrefabs orcPrefabs;
    [SerializeField] public RacePrefabs skeletonPrefabs;
    [SerializeField] public RacePrefabs witchPrefabs;

    private List<RacePrefabs> racePrefabsList;
    private Dictionary<string, RacePrefabs> selectedRaces;
    private List<GameObject> crntRace;
    public EnemySpawner2 enemySpawner=null;
    void Start()
    {
        racePrefabsList = new List<RacePrefabs> { darkElfPrefabs, humanPrefabs, orcPrefabs, skeletonPrefabs, witchPrefabs };
        selectedRaces = new Dictionary<string, RacePrefabs>();
        crntRace = new List<GameObject>();
        enemySpawner = GetComponent<EnemySpawner2>();
        SelectRandomRaceAndShowUnits();
    }

    void SelectRandomRaceAndShowUnits()
    {
        if (racePrefabsList.Count > 0)
        {
            int randomRaceIndex = Random.Range(0, racePrefabsList.Count);
            RacePrefabs selectedRace = racePrefabsList[randomRaceIndex];
            selectedRaces[selectedRace.GetType().Name] = selectedRace;
            racePrefabsList.RemoveAt(randomRaceIndex);

            Debug.Log($"Selected Race: {selectedRace.GetType().Name}");

            if (selectedRace.unitPrefabs.Length > 0)
            {
                Debug.Log("Unit Prefabs in Selected Race:");
                 foreach (GameObject unitPrefab in selectedRace.unitPrefabs)
                 {
                     Debug.Log(unitPrefab.name);
                     crntRace.Add(unitPrefab);
                 }
                
                    
                int randomUnitIndex = Random.Range(0, selectedRace.unitPrefabs.Length);
                GameObject selectedUnit = selectedRace.unitPrefabs[randomUnitIndex];

                Debug.Log($"Selected Unit: {selectedUnit.name}");
                // 여기서 selectedUnit을 사용하여 원하는 동작을 수행할 수 있습니다.

                CustomLogger.Log(crntRace);
                CustomLogger.Log(selectedUnit);
                enemySpawner.SpawnWaves(crntRace, selectedUnit);
            }
            else
            {
                Debug.Log("No unit prefabs available in selected race.");
            }
        }
        else
        {
            Debug.Log("No more races to select.");
        }
    }
}
