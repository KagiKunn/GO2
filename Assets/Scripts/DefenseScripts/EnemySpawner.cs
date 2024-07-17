using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject objectPrefab;
    
    // 오브젝트를 생성할 X축의 최소 및 최대 좌표
    public float minX = -10f;
    public float maxX = 10f;

    // 고정된 Y축 좌표
    private const float fixedY = 10.5f;

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
        for (int i = 0; i < numberOfObjects; i++)
        {
            // X축의 랜덤 좌표 생성
            float randomX = Random.Range(minX, maxX);

            // 새로운 오브젝트 생성
            Vector3 spawnPosition = new Vector3(randomX, fixedY, 0);
            GameObject enemy = Instantiate(objectPrefab, spawnPosition, Quaternion.identity, transform);

            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
