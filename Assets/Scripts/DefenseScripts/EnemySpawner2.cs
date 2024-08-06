using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner2 : MonoBehaviour
{
    // 종족별 프리팹 관리 스크립트
    [SerializeField] public DarkElfPrefabs darkElfPrefabs;
    [SerializeField] public HumanPrefabs humanPrefabs;
    [SerializeField] public OrcPrefabs orcPrefabs;
    [SerializeField] public SkeletonPrefabs skeletonPrefabs;
    [SerializeField] public WitchPrefabs witchPrefabs;

    // 스폰
    public float minY = -9f;
    public float maxY = 110f;
    private const float fixedX = 200f;
    [SerializeField] public int numberOfObjects = 10;
    private int spawnedEnemy = 0;
    public float maxSpawnInterval = 2f;

    // 웨이브
    [SerializeField] private int totalWave = 3;
    private int currentWave = 0;

    // ProgressBar 스크립트 참조
    public ProgressBar progressBar;

    void Start()
    {
        Debug.Log("EnemySpawner2: Start() 호출됨");
        // ProgressBar 초기화
        if (progressBar != null)
        {
            progressBar.SetMaxValue(numberOfObjects * totalWave);
            progressBar.SetValue(0);
        }
    }

    public IEnumerator SpawnWaves(List<GameObject> enemyPrefabs, GameObject bossPrefab)
    {
        Debug.Log("SpawnWaves() 시작됨");

        while (currentWave < totalWave)
        {
            currentWave++;
            Debug.Log(currentWave + " 웨이브 시작");

            List<GameObject> wavePrefabs = new List<GameObject>(enemyPrefabs);
            yield return StartCoroutine(SpawnObjects(wavePrefabs));

            spawnedEnemy = 0;

            if (currentWave < totalWave)
            {
                Debug.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.");
                yield return new WaitForSeconds(5f);
            }
            else
            {
                // 3번째 웨이브 종료 후 보스 소환
                Debug.Log("보스 소환");
                SpawnBoss(bossPrefab);
            }
        }
    }

    public void ResetWave()
    {
        Debug.Log("ResetWave() 호출됨");
        currentWave = 0;
    }

    private IEnumerator SpawnObjects(List<GameObject> wavePrefabs)
    {
        Debug.Log("SpawnObjects() 시작됨");
        for (int i = 0; i < numberOfObjects; i++)
        {
            float randomY = Random.Range(minY, maxY);
            GameObject randomPrefab = GetRandomPrefab(wavePrefabs);
            Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);

            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            spawnedEnemy++;
            CustomLogger.Log("생성한 적의 수 " + spawnedEnemy);

            // ProgressBar 업데이트
            if (progressBar != null)
            {
                progressBar.SetValue(spawnedEnemy + (currentWave - 1) * numberOfObjects);
            }


            if (spawnedEnemy >= numberOfObjects)
            {
                break;
            }
        }
    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        Debug.Log("SpawnBoss() 호출됨");
        if (bossPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(fixedX, (minY + maxY) / 2, 0);
            Instantiate(bossPrefab, spawnPosition, Quaternion.identity, transform);
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
