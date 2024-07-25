using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefenseScripts;

public class EnemySpawner2 : MonoBehaviour
{
    // 스테이지별 적 프리팹 목록을 저장하는 리스트
    [SerializeField] private List<EnemyStage> stageEnemyPrefabs;

    // 현재 스테이지 번호
    [SerializeField] public int currentStage = 1;

    // 웨이브에서 사용할 적 프리팹 목록
    private List<GameObject> _enemyPrefabs;

    // 오브젝트를 생성할 Y축의 최소 및 최대 좌표
    public float minY = -0f;
    public float maxY = 10f;

    // 고정된 X축 좌표
    private const float fixedX = 20f;

    // 생성할 오브젝트의 개수
    [SerializeField] public int numberOfObjects = 10;

    // 스포너가 생성한 적 오브젝트 개수
    private int spawnedEnemy = 0;

    // 최대 생성 간격
    public float maxSpawnInterval = 2f;

    // 총 웨이브 수
    [SerializeField] private int totalWave = 3;
    
    // 현재 웨이브
    private int currentWave = 0;

    void Start()
    {
        CustomLogger.Log("Current Stage: "+currentStage, "yellow");
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        // 스테이지별 적 prefab가 저장된 배열 stageEnemyPrefabs[]에서
        // 그 인덱스번호에 해당하는 종족을 가져옴
        EnemyStage stage = stageEnemyPrefabs[currentStage - 1];
        
        //이번 스테이지에서 사용할 적 Prefab 배열을 초기화
        _enemyPrefabs = new List<GameObject>();

        // 현재 스테이지 번호에 따라 적 프리팹 배열을 설정
        switch (currentStage)
        {
            case 1:
                _enemyPrefabs.AddRange(stage.darkElf);
                break;
            case 2:
                _enemyPrefabs.AddRange(stage.human);
                break;
            case 3:
                _enemyPrefabs.AddRange(stage.orc);
                break;
            case 4:
                _enemyPrefabs.AddRange(stage.skeleton);
                break;
            case 5:
                _enemyPrefabs.AddRange(stage.witch);
                break;
        }

        //Waveによる敵兵組み合わせ調整
        while (currentWave < totalWave)
        {
            currentWave++;
            
            //결정된 스테이지 프리팹을 사용해서 Wave프리팹을 초기화
            List<GameObject> wavePrefabs = new List<GameObject>(_enemyPrefabs);

            //
            switch (currentWave)
            {
                //1웨이브일경우: 프리팹이 2개 이상일 경우(예외처리) 0 1 2 3 4  
                case 1:
                    if (wavePrefabs.Count > 2)
                    {
                        wavePrefabs.RemoveRange(2, wavePrefabs.Count - 2); // 앞의 2개만 사용
                    }
                    break;
                case 2:
                    if (wavePrefabs.Count > 4)
                    {
                        wavePrefabs.RemoveRange(4, wavePrefabs.Count - 4); // 앞의 4개만 사용
                    }
                    break;
                case 3:
                    // 모든 프리팹을 사용
                    break;
            }

            CustomLogger.Log(currentWave + "웨이브 시작");
            yield return StartCoroutine(SpawnObjects(wavePrefabs));

            spawnedEnemy = 0; // 웨이브 종료 시 초기화

            if (currentWave < totalWave)
            {
                CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.", "yellow");
                yield return new WaitForSeconds(5f);
            }
        }

        CustomLogger.Log("모든 웨이브가 완료되었습니다.", "red");
    }

    IEnumerator SpawnObjects(List<GameObject> wavePrefabs)
    {
        
        for (int i = 0; i < numberOfObjects; i++)
        {
            
            float randomY = Random.Range(minY, maxY);

            // Random Prefab 選択
            GameObject randomPrefab = GetRandomPrefab(wavePrefabs);

            Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);

            float waitTime = Random.Range(0, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            spawnedEnemy++;
            CustomLogger.Log("생성한 적의 수 " + spawnedEnemy);

            // 最大数に達するとWave終了。
            if (spawnedEnemy >= numberOfObjects)
            {
                break;
            }
        }
    }

    /*確率の区間を０～１までの範囲で分けるイメージ。
       randomValueが任意の数を,0f～1fまでの間で引く
       例えばfloat[] { 0.35f, 0.35f, 0.15f, 0.15f };の場合
       この確率テーブルの配列を巡回しながらその数がどの区間に位置するかを判定する。*/
    GameObject GetRandomPrefab(List<GameObject> wavePrefabs)
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

    float[] GetPercentages(int prefabCount)
    {
        //現在のWaveによって出現率を調整する。
        switch (currentWave)
        {
            case 1: //1Wave
                return new float[] { 0.5f, 0.5f }; // 各50%
            case 2:
                return new float[] { 0.35f, 0.35f, 0.15f, 0.15f }; // 35%, 35%, 15%, 15%
            case 3:
                return new float[] { 0.25f, 0.25f, 0.2f, 0.2f, 0.1f }; // 25%, 25%, 20%, 20%, 10%
            default:
                return new float[] { 1f }; // 100%
        }
    }
}