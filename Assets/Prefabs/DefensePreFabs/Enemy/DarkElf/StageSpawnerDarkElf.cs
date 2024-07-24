using UnityEngine;
using System.Collections;

public class StageSpawnerDarkElf : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] wave1Enemy;
    public GameObject[] wave2Enemy;
    public GameObject[] wave3Enemy;
    
    // 오브젝트를 생성할 X축의 최소 및 최대 좌표
    public float minY = -0f;
    public float maxY = 10f;

    // 고정된 Y축 좌표
    private const float fixedX = 20f;

    // 생성할 오브젝트의 개수
    [SerializeField]
    public int numberOfObjects = 10;

    // 최대 시간 간격
    public float maxSpawnInterval = 2f;
    
    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        //오브젝트 최대수 제한까지 반복생성
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Y축의 랜덤 좌표 생성
            float randomY = Random.Range(minY, maxY);

            // 새로운 오브젝트 생성
            GameObject randomPrefab = GetRandomPrefab();
            
            Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
            GameObject enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);

            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    GameObject GetRandomPrefab()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < 0.25f)
        {
            return wave3Enemy[0]; // 25%
        }
        else if (randomValue < 0.50f)
        {
            return wave3Enemy[1]; // 25%
        }
        else if (randomValue < 0.70f)
        {
            return wave3Enemy[2]; // 20%
        }
        else if (randomValue < 0.90f)
        {
            return wave3Enemy[3]; // 20%
        }
        else
        {
            return wave3Enemy[4]; // 10%
        }
    }
}