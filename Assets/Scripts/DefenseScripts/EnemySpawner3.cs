using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefenseScripts;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemySpawner3 : MonoBehaviour
{
    [SerializeField] private StageController stageController;

    public float minY = -0f;
    public float maxY = 10f;
    private const float fixedX = 20f;
    [SerializeField] public int numberOfObjects = 10;
    private int spawnedEnemy = 0;
    public float maxSpawnInterval = 2f;
    [SerializeField] public float spawnInterval = 10f;

    public ProgressBar progressBar;
    public GameObject stageEndPopup;
    public Button nextTurnButton;

    private void Start()
    {
        // 스테이지 진행도 바
        if (progressBar != null)
        {
            progressBar.SetMaxValue(numberOfObjects * stageController.TotalWave);
            progressBar.SetValue(0);
        }

        // 다음 턴으로 가는 버튼
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
        }

        StartCoroutine(ManageStages());
    }

    // 스테이지 관리
    IEnumerator ManageStages()
    {
        while (stageController.CurrentStage <= 5)
        {
            CustomLogger.Log("Current Stage: " + stageController.CurrentStage, "yellow");

            yield return StartCoroutine(SpawnWaves());
            CustomLogger.Log("스테이지 종료", "red");

            // 스테이지 종료 시 팝업 UI 활성화
            if (stageEndPopup != null)
            {
                stageEndPopup.SetActive(true);
            }

            // 팝업에서 버튼이 클릭될 때까지 대기
            while (stageEndPopup.activeSelf)
            {
                yield return null;
            }

            stageController.IncrementStage();
        }

        CustomLogger.Log("모든 스테이지 완료", "red");
    }

    // 웨이브 관리
    IEnumerator SpawnWaves()
    {
        // 적 프리팹이 선택되지 않았음을 검증
        if (stageController.EnemyPrefabs == null || stageController.EnemyPrefabs.Count == 0)
        {
            // 적 프리팹이 선택되지 않았으면 이걸 호출해서 적 리스트를 선택
            stageController.SelectRandomEnemyPrefabs();
        }

        // 전체 웨이브보다 현재 웨이브 카운트가 적을 경우에만 반복 스폰
        while (stageController.CurrentWave < stageController.TotalWave)
        {
            stageController.IncrementWave();

            List<GameObject> wavePrefabs = new List<GameObject>(stageController.EnemyPrefabs);
            CustomLogger.Log(stageController.CurrentWave + "웨이브 시작");
            yield return StartCoroutine(SpawnObjects(wavePrefabs));

            spawnedEnemy = 0;

            if (stageController.CurrentWave < stageController.TotalWave)
            {
                CustomLogger.Log("웨이브 " + stageController.CurrentWave + " 종료. 다음 웨이브까지 " + spawnInterval + "초 대기", "yellow");
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    // 적 스폰 담당
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

            // 스테이지 진행 UI바 값 조정
            if (progressBar != null)
            {
                progressBar.SetValue(spawnedEnemy + (stageController.CurrentWave - 1) * numberOfObjects);
            }

            if (spawnedEnemy >= numberOfObjects)
            {
                break;
            }
        }
    }

    // 적 병종 랜덤 생성
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

    // 병종 조합별 확률 설정
    float[] GetPercentages(int prefabCount)
    {
        switch (stageController.CurrentWave)
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

    // 버튼 클릭 시 호출
    void OnNextTurnButtonClicked()
    {
        if (stageEndPopup != null)
        {
            stageEndPopup.SetActive(false);
        }

        // InternalAffairs 씬으로 전환
        SceneManager.LoadScene("InternalAffairs");
    }
}