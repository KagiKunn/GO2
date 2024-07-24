using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] objectPrefabs;

    // 오브젝트를 생성할 X축의 최소 및 최대 좌표
    public float minY = -0f;
    public float maxY = 10f;

    // 고정된 Y축 좌표
    private const float fixedX = 20f;

    // 생성할 오브젝트의 개수
    [SerializeField] public int numberOfObjects = 10;

    // 스포너가 생성한 적 오브젝트 개수
    private int spawnedEnemy = 0;
    // 최대 시간 간격
    public float maxSpawnInterval = 2f;

    [SerializeField] private int totalWave = 3;
    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves()); // 이전: StartCoroutine(SpawnObjects());
    }

    // 웨이브 조정
    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWave)
        {
            currentWave++;
            CustomLogger.Log(currentWave + "웨이브 시작");
            yield return StartCoroutine(SpawnObjects());

            spawnedEnemy = 0; // 웨이브 종료 시 초기화

            if (currentWave <= totalWave)
            {
                CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.", "yellow");
                yield return new WaitForSeconds(10f);
            }
        }

        CustomLogger.Log("모든 웨이브가 완료되었습니다.", "red");
    }
    
    IEnumerator SpawnObjects()
    {
        //오브젝트 최대수 제한까지 반복생성
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Y축의 랜덤 좌표 생성
            float randomY = Random.Range(minY, maxY);

            // 새로운 오브젝트 생성
            int randomIndex = Random.Range(0, objectPrefabs.Length);
            GameObject randomPrefab = GetRandomPrefab();

            Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
            GameObject enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);

            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            spawnedEnemy++;
            CustomLogger.Log("생성한 적의 수 " + spawnedEnemy);
            
            //최대 생성수에 도달하면 웨이브를 종료
            if (spawnedEnemy == numberOfObjects)
            {
                break;
            }
        }
        
    }

    GameObject GetRandomPrefab()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < 0.25f)
        {
            return objectPrefabs[0]; // 25%
        }
        else if (randomValue < 0.50f)
        {
            return objectPrefabs[1]; // 25%
        }
        else if (randomValue < 0.70f)
        {
            return objectPrefabs[2]; // 20%
        }
        else if (randomValue < 0.90f)
        {
            return objectPrefabs[3]; // 20%
        }
        else
        {
            return objectPrefabs[4]; // 10%
        }
    }
}