using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private RacePrefabs darkElfPrefabs;
    [SerializeField] private RacePrefabs humanPrefabs;
    [SerializeField] private RacePrefabs orcPrefabs;
    [SerializeField] private RacePrefabs skeletonPrefabs;
    [SerializeField] private RacePrefabs witchPrefabs;

    private List<RacePrefabs> racePrefabsList;
    private List<GameObject> currentRacePrefabs;
    public EnemySpawner2 enemySpawner;

    void Start()
    {
        racePrefabsList = new List<RacePrefabs> { darkElfPrefabs, humanPrefabs, orcPrefabs, skeletonPrefabs, witchPrefabs };
        currentRacePrefabs = new List<GameObject>();
        enemySpawner = GetComponent<EnemySpawner2>();
        SelectRandomRace();
    }

    void SelectRandomRace()
    {
        if (racePrefabsList.Count > 0)
        {
            int randomRaceIndex = Random.Range(0, racePrefabsList.Count);
            RacePrefabs selectedRace = racePrefabsList[randomRaceIndex];
            racePrefabsList.RemoveAt(randomRaceIndex);

            Debug.Log($"Selected Race: {selectedRace.GetType().Name}");

            currentRacePrefabs = new List<GameObject>(selectedRace.unitPrefabs);
            enemySpawner.Initialize(currentRacePrefabs, selectedRace.bossPrefab);
        }
        else
        {
            Debug.Log("No more races to select.");
        }
    }
}