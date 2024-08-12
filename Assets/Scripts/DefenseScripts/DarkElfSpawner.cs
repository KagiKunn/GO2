using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.

public class DarkElfSpawner : MonoBehaviour
{
    // 스테이지 종족의 모든 적 병사를 넣는 배열
    [SerializeField] private List<GameObject> allEnemyPrefabs;

    // 웨이브에서 사용할 배열
    private List<GameObject> _enemyPrefabs;

    // 오른쪽 스폰 위치의 Y축 최소 및 최대 좌표
    public float rightMinY = -20f;
    public float rightMaxY = 120f;

    // 왼쪽 스폰 위치의 Y축 최소 및 최대 좌표
    private float leftMinY = -280f;
    private float leftMaxY = -420f;

    // 고정된 X축 좌표
    private const float rightX = -30f;
    private const float leftX = 150f;

    // 생성할 오브젝트의 개수
    [SerializeField] public int numberOfObjects = 10;

    // 스포너가 생성한 적 오브젝트 개수
    private int spawnedEnemy = 0;

    // 최대 시간 간격
    public float maxSpawnInterval = 2f;

    [SerializeField] private int totalWave = 3;
    private int currentWave = 0;

    // 보스 프리팹
    [SerializeField] private GameObject bossPrefab;

    // ProgressBar
    public ProgressBar progressBar;
    private int totalEnemiesToSpawn;

    public bool rightSpawn;
    
    void Start()
    {
        // 총 적 수 계산
        totalEnemiesToSpawn = numberOfObjects * totalWave;

        // ProgressBar 초기화
        if (progressBar != null)
        {
            progressBar.SetMaxValue(totalEnemiesToSpawn);
            progressBar.SetValue(0);
        }

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWave)
        {
            currentWave++;
            _enemyPrefabs = new List<GameObject>(); // 웨이브마다 List 초기화

            switch (currentWave)
            {
                case 1:
                    _enemyPrefabs.Add(allEnemyPrefabs[0]);
                    _enemyPrefabs.Add(allEnemyPrefabs[1]);
                    break;
                case 2:
                    _enemyPrefabs.Add(allEnemyPrefabs[0]);
                    _enemyPrefabs.Add(allEnemyPrefabs[1]);
                    _enemyPrefabs.Add(allEnemyPrefabs[2]);
                    _enemyPrefabs.Add(allEnemyPrefabs[3]);
                    break;
                case 3:
                    _enemyPrefabs.Add(allEnemyPrefabs[0]);
                    _enemyPrefabs.Add(allEnemyPrefabs[1]);
                    _enemyPrefabs.Add(allEnemyPrefabs[2]);
                    _enemyPrefabs.Add(allEnemyPrefabs[3]);
                    _enemyPrefabs.Add(allEnemyPrefabs[4]);
                    break;
            }

            CustomLogger.Log(currentWave + " 웨이브 시작");

            yield return StartCoroutine(SpawnObjects());

            // 웨이브 종료 시 적 수 초기화
            spawnedEnemy = 0;

            if (currentWave <= totalWave)
            {
                CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 10초 대기.", "yellow");
                yield return new WaitForSeconds(10f);
            }
        }

        // 모든 웨이브가 끝난 후 보스 생성
        CustomLogger.Log("모든 웨이브가 완료되었습니다. 보스를 소환합니다.", "red");
        SpawnBoss();
    }

    IEnumerator SpawnObjects()
    {
        // 오브젝트 최대수 제한까지 반복 생성
        for (int i = 0; i < numberOfObjects; i++)
        {
            // rightSpawn 값을 랜덤으로 결정
            rightSpawn = Random.value > 0.5f;
            
            // Y축의 랜덤 좌표 생성
            float randomY = rightSpawn ? Random.Range(rightMinY, rightMaxY) : Random.Range(leftMinY, leftMaxY);

            // 새로운 오브젝트 생성
            GameObject randomPrefab = GetRandomPrefab();

            Vector3 spawnPosition = rightSpawn ? new Vector3(rightX, randomY, 0) : new Vector3(leftX, randomY, 0);

            // 적 생성 및 방향 설정
            GameObject enemy;
            if (rightSpawn)
            {
                enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);
                enemy.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = true;  // 오른쪽에서 스폰되었으므로 isRight를 true로 설정
            }
            else
            {
                enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.Euler(0, 180, 0), transform);
                enemy.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = false;  // 왼쪽에서 스폰되었으므로 isRight를 false로 설정
            }

            float waitTime = Random.Range(0, maxSpawnInterval);

            yield return new WaitForSeconds(waitTime);

            spawnedEnemy++;
            CustomLogger.Log("생성한 적의 수: " + spawnedEnemy, "yellow");

            // ProgressBar 값 업데이트
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

    GameObject GetRandomPrefab()
    {
        float randomValue = Random.Range(0f, 1f);
        float[] percentages = GetPercentages();

        for (int i = 0; i < percentages.Length; i++)
        {
            if (randomValue < percentages[i])
            {
                return _enemyPrefabs[i];
            }

            randomValue -= percentages[i];
        }

        return _enemyPrefabs[_enemyPrefabs.Count - 1];
    }

    float[] GetPercentages()
    {
        switch (currentWave)
        {
            case 1:
                return new float[] { 0.5f, 0.5f }; // 각 50%
            case 2:
                return new float[] { 0.35f, 0.35f, 0.15f, 0.15f }; // 35% 35% 15% 15%
            case 3:
                return new float[] { 0.25f, 0.25f, 0.2f, 0.2f, 0.1f }; // 25% 25% 20% 20% 10%
            default:
                return new float[] { 1f };
        }
    }

    void SpawnBoss()
    {
        // rightSpawn 값을 랜덤으로 결정
        rightSpawn = Random.value > 0.5f;

        float randomY = rightSpawn ? Random.Range(rightMinY, rightMaxY) : Random.Range(leftMinY, leftMaxY);
        Vector3 spawnPosition = rightSpawn ? new Vector3(rightX, randomY, 0) : new Vector3(leftX, randomY, 0);
        GameObject boss;
        if (rightSpawn)
        {
            boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity, transform);
            boss.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = true; 
        }
        else
        {
            boss =Instantiate(bossPrefab, spawnPosition, Quaternion.Euler(0, 180, 0), transform);
            boss.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = false;
        }

        // boss.transform.localScale *= 3;
    }
}