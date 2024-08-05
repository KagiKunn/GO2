using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] private int totalStages = 5;
    [SerializeField] private EnemySpawner2 enemySpawner;
    public int CurrentStage { get; private set; } = 1;

    private List<int> usedStages = new List<int>();
    private List<string> usedRaces = new List<string>();
    private GameObject bossPrefab;
    private List<GameObject> enemyPrefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("StageManager: Start() 호출됨");
        StartCoroutine(ManageStages());
    }

    private IEnumerator ManageStages()
    {
        Debug.Log("ManageStages() 시작됨");
        while (CurrentStage <= totalStages)
        {
            CustomLogger.Log("Current Stage: " + CurrentStage, "yellow");

            if (!usedStages.Contains(CurrentStage))
            {
                SelectRandomEnemyPrefabs(CurrentStage);
                usedStages.Add(CurrentStage);
            }

            Debug.Log("Spawning waves for stage " + CurrentStage);
            yield return StartCoroutine(enemySpawner.SpawnWaves(enemyPrefabs, bossPrefab));
            CustomLogger.Log("스테이지 종료", "red");
            yield return new WaitForSeconds(5f);

            CurrentStage++;
            enemySpawner.ResetWave();
        }

        CustomLogger.Log("모든 스테이지가 완료되었습니다.", "red");
    }

    private void SelectRandomEnemyPrefabs(int currentStage)
    {
        Debug.Log("SelectRandomEnemyPrefabs() 호출됨");
        List<GameObject> selectedEnemies = new List<GameObject>();
        List<string> availableRaces = new List<string> { "darkElf", "human", "orc", "skeleton", "witch" };
        availableRaces.RemoveAll(race => usedRaces.Contains(race));

        if (availableRaces.Count == 0)
        {
            CustomLogger.Log("모든 종족이 이미 사용되었습니다.", "red");
            enemyPrefabs = selectedEnemies;
            return;
        }

        string selectedRace = availableRaces[Random.Range(0, availableRaces.Count)];
        usedRaces.Add(selectedRace);
        CustomLogger.Log("선택된 종족 : " + selectedRace, "yellow");

        switch (selectedRace)
        {
            case "darkElf":
                selectedEnemies.AddRange(enemySpawner.darkElfPrefabs.GetPrefabs());
                bossPrefab = enemySpawner.darkElfPrefabs.GetBossPrefab();
                break;
            case "human":
                selectedEnemies.AddRange(enemySpawner.humanPrefabs.GetPrefabs());
                bossPrefab = enemySpawner.humanPrefabs.GetBossPrefab();
                break;
            case "orc":
                selectedEnemies.AddRange(enemySpawner.orcPrefabs.GetPrefabs());
                bossPrefab = enemySpawner.orcPrefabs.GetBossPrefab();
                break;
            case "skeleton":
                selectedEnemies.AddRange(enemySpawner.skeletonPrefabs.GetPrefabs());
                bossPrefab = enemySpawner.skeletonPrefabs.GetBossPrefab();
                break;
            case "witch":
                selectedEnemies.AddRange(enemySpawner.witchPrefabs.GetPrefabs());
                bossPrefab = enemySpawner.witchPrefabs.GetBossPrefab();
                break;
        }

        enemyPrefabs = selectedEnemies;
        Debug.Log("Selected enemy prefabs count: " + enemyPrefabs.Count);
        Debug.Log("Selected boss prefab: " + (bossPrefab != null ? bossPrefab.name : "null"));
    }
}
