using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefenseScripts;

public class EnemySpawner2 : MonoBehaviour
{
    //스테이지 
    [SerializeField] private List<EnemyStage> stageEnemyPrefabs;
    [SerializeField] public int currentStage = 1;
    private List<GameObject> enemyPrefabs;
    private List<int> usedStages = new List<int>();
    private List<string> usedRaces = new List<string>();
    
    //스폰
    public float minY = -0f;
    public float maxY = 10f;
    private const float fixedX = 20f;
    [SerializeField] public int numberOfObjects = 10;
    private int spawnedEnemy = 0;
    public float maxSpawnInterval = 2f;
    
    //웨이브
    [SerializeField] private int totalWave = 3;
    private int currentWave = 0;

    //ProgressBar 스크립트 참조
    public ProgressBar progressBar; 
    

    void Start()
    {
        // ProgressBar 초기화
        if (progressBar != null)
        {
            //MaxValue　=　このステージで生成される敵の総数
            progressBar.SetMaxValue(numberOfObjects * totalWave);
            progressBar.SetValue(0);
        }

        StartCoroutine(ManageStages());
    }

    //ステージ決定
    IEnumerator ManageStages()
    {
        while (currentStage <= 5)
        {
            CustomLogger.Log("Current Stage: " + currentStage, "yellow");
            
            //SpawnWaves()でWave生成が終わるまで保留、
            //３Waveまで進行した後に実行を再開
            yield return StartCoroutine(SpawnWaves());
            CustomLogger.Log("스테이지 종료", "red");
            yield return new WaitForSeconds(5f);

            currentStage++;
            currentWave = 0;
        }

        CustomLogger.Log("모든 스테이지가 완료되었습니다.", "red");
    }

    IEnumerator SpawnWaves()
    {
        // 각 스테이지마다 등장할 종족을 랜덤하게 선택
        if (!usedStages.Contains(currentStage))
        {
            enemyPrefabs = SelectRandomEnemyPrefabs();
            usedStages.Add(currentStage);
        }

        while (currentWave < totalWave)
        {
            currentWave++;
            
            List<GameObject> wavePrefabs = new List<GameObject>(enemyPrefabs);

            CustomLogger.Log(currentWave + "웨이브 시작");
            yield return StartCoroutine(SpawnObjects(wavePrefabs));

            spawnedEnemy = 0;

            if (currentWave < totalWave)
            {
                CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.", "yellow");
                yield return new WaitForSeconds(5f);
            }
        }
    }

    //ステージごとランダムで種族を選び、その種族のPrefabをreturn
    List<GameObject> SelectRandomEnemyPrefabs()
    {
        //最終的にreturnされる敵兵PrefabのList宣言
        List<GameObject> selectedEnemies = new List<GameObject>();
        
        //使用可能な種族のListを作成+usedRacesに含まれてる種族を削除
        //今回のステージで使用可能な種族だけがList内に残る。
        List<string> availableRaces = new List<string> { "darkElf", "human", "orc", "skeleton", "witch" };
        availableRaces.RemoveAll(race => usedRaces.Contains(race));

        //使用可能な種族が残っていない場合の例外処理
        if (availableRaces.Count == 0)
        {
            CustomLogger.Log("모든 종족이 이미 사용되었습니다.", "red");
            return selectedEnemies;
        }

        //availableRaves[]の中からランダムで選んで、usedRacesに追加する。
        string selectedRace = availableRaces[Random.Range(0, availableRaces.Count)];
        usedRaces.Add(selectedRace);
        CustomLogger.Log("선택된 종족 : "+selectedRace,"yellow");

        //選ばれた種族のPrefabのListを持ち込んで、selectedEnemiesに入れてreturn
        EnemyStage stage = stageEnemyPrefabs[currentStage - 1];

        switch (selectedRace)
        {
            case "darkElf":
                selectedEnemies.AddRange(stage.darkElf);
                break;
            case "human":
                selectedEnemies.AddRange(stage.human);
                break;
            case "orc":
                selectedEnemies.AddRange(stage.orc);
                break;
            case "skeleton":
                selectedEnemies.AddRange(stage.skeleton);
                break;
            case "witch":
                selectedEnemies.AddRange(stage.witch);
                break;
        }

        return selectedEnemies;
    }

    IEnumerator SpawnObjects(List<GameObject> wavePrefabs)
    {
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