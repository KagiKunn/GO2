using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2 : MonoBehaviour
{
    public float minY = -9f;
    public float maxY = 10f;
    private const float fixedX = 20f;
    [SerializeField] private int numberOfObjects = 10;
    public float maxSpawnInterval = 2f;
    [SerializeField] private int totalWave = 3;
    private int currentWave = 0;
    private List<GameObject> enemyPrefabs;
    private GameObject bossPrefab;
    public ProgressBar progressBar;
    public GameObject stageEndUI;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.SetMaxValue(numberOfObjects * totalWave);
            progressBar.SetValue(0);
        }
        else
        {
            Debug.LogWarning("ProgressBar가 설정되지 않았습니다.");
        }

        if (stageEndUI != null)
        {
            stageEndUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("StageEndUI가 설정되지 않았습니다.");
        }
    }

    public void Initialize(List<GameObject> enemyPrefabsList, GameObject bossPrefab)
    {
        enemyPrefabs = new List<GameObject>(enemyPrefabsList);
        this.bossPrefab = bossPrefab;
        currentWave = 0; // Initialize currentWave before starting
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < totalWave)
        {
            currentWave++;
            Debug.Log($"{currentWave} 웨이브 시작");

            List<GameObject> wavePrefabs = new List<GameObject>(enemyPrefabs);
            yield return StartCoroutine(SpawnObjects(wavePrefabs));

            if (progressBar != null)
            {
                progressBar.SetValue(currentWave * numberOfObjects);
            }

            if (currentWave < totalWave)
            {
                Debug.Log($"웨이브 {currentWave} 종료. 다음 웨이브까지 5초 대기.");
                yield return new WaitForSeconds(5f);
            }
        }

        // 모든 웨이브가 종료된 후 보스 소환
        Debug.Log("모든 웨이브가 완료되었습니다. 보스 소환");
        SpawnBoss();
    }

    private IEnumerator SpawnObjects(List<GameObject> wavePrefabs)
    {
        Debug.Log("SpawnObjects() 시작됨");

        for (int i = 0; i < numberOfObjects; i++)
        {
            float randomY = Random.Range(minY, maxY);
            GameObject randomPrefab = GetRandomPrefab(wavePrefabs);
            Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("적 소환됨");
            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(fixedX, (minY + maxY) / 2, 0);
            GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
            boss.GetComponent<Boss>().OnBossDefeated += OnBossDefeated;
        }
        else
        {
            Debug.LogWarning("bossPrefab이 null입니다.");
        }
    }

    private void OnBossDefeated()
    {
        Debug.Log("보스가 쓰러졌습니다.");

        if (stageEndUI != null)
        {
            stageEndUI.SetActive(true);
        }
    }

    private GameObject GetRandomPrefab(List<GameObject> wavePrefabs)
    {
        float randomValue = Random.Range(0f, 1f);
        float[] percentages = GetPercentages(wavePrefabs.Count);

        for (int i = 0; i < percentages.Length; i++)
        {
            if (randomValue < percentages[i])
            {
                return wavePrefabs[i];
            }

            randomValue -= percentages[i];
        }

        return wavePrefabs[wavePrefabs.Count - 1];
    }

    private float[] GetPercentages(int prefabCount)
    {
        switch (currentWave)
        {
            case 1:
                return new float[] { 0.5f, 0.5f };
            case 2:
                return new float[] { 0.35f, 0.35f, 0.15f, 0.15f };
            case 3:
                return new float[] { 0.25f, 0.25f, 0.2f, 0.2f, 0.1f };
            default:
                return new float[] { 1f };
        }
    }
}
