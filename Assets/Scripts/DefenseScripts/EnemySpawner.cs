using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class EnemySpawner : MonoBehaviour
{
    // EnemyPrefabList 스크립트 참조
    private EnemyPrefabList enemyPrefabList;

    // 웨이브에서 사용할 배열
    private List<GameObject> _enemyPrefabs;

    // 오른쪽 스폰 위치의 Y축 최소 및 최대 좌표
    public float rightMinY = -20f;
    public float rightMaxY = 115f;

    // 왼쪽 스폰 위치의 Y축 최소 및 최대 좌표
    private float leftMinY = -280f;
    private float leftMaxY = -415f;

    // 고정된 X축 좌표
    private const float rightX = 200f;
    private const float leftX = -100f;

    // 생성할 오브젝트의 개수
    [SerializeField] public int numberOfObjects = 1;

    // 스포너가 생성한 적 오브젝트 개수
    private int spawnedEnemy = 0;

    // 최대 시간 간격
    public float maxSpawnInterval = 2f;

    [SerializeField] private int totalWave = 3;
    public int currentWave = 0;

    // 보스 프리팹은 EnemyPrefabList의 마지막 인덱스로 설정
    private GameObject bossPrefab;

    // ProgressBar
    public ProgressBar progressBar;
    private int totalEnemiesToSpawn;

    public bool rightSpawn;

    // StageC 에서 설정된 종족명
    [SerializeField] private string selectedRace;

    //스테이지 정보 참조
    [SerializeField] public int stageCount;

    [SerializeField] public int weekCount;

    // 현재 웨이브에서 사망한 적의 수
    public int enemyDieCount = 0;

    // 전체에서 사망한 적의 수
    public int totalEnemyDieCount = 0;

    public GameObject bossImage;  // 보스 소환 전 나타날 이미지
    public float displayTime = 4.0f;  // 이미지가 표시될 시간(초)
    private float blinkDuration = 4.0f;  // 깜빡임이 지속될 시간(초)
    private float blinkInterval = 0.3f;  // 깜빡임 간격(초)
    
    public void SetSelectedRace(string race)
    {
        selectedRace = race;
        CustomLogger.Log("에너미 스포너의 SetSelectedRace : " + selectedRace, "pink");
    }

    private void Start()
    {
        CustomLogger.Log("EnemySpawner Start()진입", "pink");
        bossImage.SetActive(false);
        
        //스테이지 수에 따른 웨이브당 스폰 숫자 증가
        numberOfObjects *= stageCount;
        CustomLogger.Log("StageCount를 받아와서 스폰할 숫자 재설정 결과 : " + numberOfObjects, "pink");

        // EnemyPrefabList 참조 가져오기
        enemyPrefabList = FindObjectOfType<EnemyPrefabList>();

        if (enemyPrefabList == null)
        {
            Debug.LogError("EnemyPrefabList를 찾을 수 없습니다!");

            return;
        }

        // 선택된 종족의 적 프리팹 배열 가져오기
        var enemyPrefabsArray = enemyPrefabList.GetEnemyPrefabs(selectedRace);
        CustomLogger.Log("적 프리팹 배열 길이: " + enemyPrefabsArray.Length, "pink");

        if (enemyPrefabsArray.Length > 0)
        {
            _enemyPrefabs = new List<GameObject>(enemyPrefabsArray);
            CustomLogger.Log("_enemyPrefabs 초기화 완료: " + _enemyPrefabs.Count + "개의 프리팹이 추가됨", "pink");

            bossPrefab = enemyPrefabsArray[enemyPrefabsArray.Length - 1];
            CustomLogger.Log("보스 프리팹 설정 완료: " + bossPrefab.name, "pink");
        }
        else
        {
            Debug.LogError("EnemyPrefabList에서 프리팹을 찾을 수 없습니다!");

            return;
        }

        // 총 적 수 계산
        totalEnemiesToSpawn = numberOfObjects * totalWave;

        // ProgressBar 초기화
        if (progressBar != null)
        {
            progressBar.SetMaxValue(totalEnemiesToSpawn);
            progressBar.SetValue(0);
        }

        CustomLogger.Log("SpawnWaves() 시작", "pink");
        StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        if (CheckBossSpawn())
        {
            CustomLogger.Log("모든 적이 사망했습니다. 보스를 소환합니다.", "red");
            StartCoroutine(DisplayBossImageAndSpawn());
            totalEnemyDieCount = 0;
        }
    }

    private IEnumerator DisplayBossImageAndSpawn()
    {
        bossImage.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            // 이미지 깜빡이기 (활성화/비활성화 반복)
            bossImage.SetActive(!bossImage.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }
        
        bossImage.SetActive(false);

        SpawnBoss();
    }

    IEnumerator SpawnWaves()
    {
        CustomLogger.Log("SpawnWaves()입갤 ㅋ");

        while (currentWave < totalWave)
        {
            CustomLogger.Log("While문 입장?");
            currentWave++;

            // 현재 웨이브에 맞는 적 프리팹들을 가져옴
            _enemyPrefabs.AddRange(GetWaveEnemyPrefabs());

            CustomLogger.Log(currentWave + " 웨이브 시작. 적 프리팹 수: " + _enemyPrefabs.Count, "pink");

            if (_enemyPrefabs.Count > 0)
            {
                yield return StartCoroutine(SpawnObjects());
            }
            else
            {
                CustomLogger.LogError("적 프리팹이 초기화되지 않았습니다!", "red");
            }

            // 모든 적이 스폰된 후에 체크
            if (spawnedEnemy >= numberOfObjects) {
                // 적이 모두 제거될 때까지 대기
                while (enemyDieCount < spawnedEnemy) {
                    yield return null; // 적들이 모두 죽을 때까지 대기
                }
            }
            
            // 웨이브 종료 시 적 수 초기화
            spawnedEnemy = 0;
            enemyDieCount = 0;
            
            if (currentWave < totalWave)
            {
                CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 10초 대기.", "yellow");

                yield return new WaitForSeconds(5f);
            }
        }
    }

    public bool CheckBossSpawn()
    {
        if (totalEnemiesToSpawn == totalEnemyDieCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator SpawnObjects()
    {
        CustomLogger.Log("SpawnObjects() 시작. 생성할 적 수: " + numberOfObjects, "pink");

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
                enemy.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = true;
            }
            else
            {
                enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.Euler(0, 180, 0), transform);
                enemy.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = false;
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

    List<GameObject> GetWaveEnemyPrefabs()
    {
        CustomLogger.Log("GetWaveEnemyPrefabs() 진입");
        CustomLogger.Log("_enemyPrefabs의 프리팹 수: " + _enemyPrefabs.Count, "pink");
        CustomLogger.Log("현재 웨이브: " + currentWave, "pink");

        List<GameObject> wavePrefabs = new List<GameObject>();

        switch (currentWave)
        {
            case 1:
                wavePrefabs.Add(_enemyPrefabs[0]);
                wavePrefabs.Add(_enemyPrefabs[1]);

                break;
            case 2:
                wavePrefabs.Add(_enemyPrefabs[0]);
                wavePrefabs.Add(_enemyPrefabs[1]);
                wavePrefabs.Add(_enemyPrefabs[2]);
                wavePrefabs.Add(_enemyPrefabs[3]);

                break;
            case 3:
                wavePrefabs.Add(_enemyPrefabs[0]);
                wavePrefabs.Add(_enemyPrefabs[1]);
                wavePrefabs.Add(_enemyPrefabs[2]);
                wavePrefabs.Add(_enemyPrefabs[3]);
                wavePrefabs.Add(_enemyPrefabs[4]);

                break;
            default:
                CustomLogger.LogError("currentWave 값이 범위를 벗어났습니다: " + currentWave, "red");

                break;
        }

        CustomLogger.Log("GetWaveEnemyPrefabs() - Wave " + currentWave + "의 적 프리팹 수: " + wavePrefabs.Count, "pink");

        return wavePrefabs;
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
            boss = Instantiate(bossPrefab, spawnPosition, Quaternion.Euler(0, 180, 0), transform);
            boss.transform.GetChild(0).GetComponent<EnemyMovement>().isRight = false;
        }

        float scaleMultiplier = 3f;
        boss.transform.localScale *= scaleMultiplier;

        Vector3 scaleCorrection = Vector3.one * (scaleMultiplier - 1) * 0.5f;
        boss.transform.position -= scaleCorrection;
    }
}