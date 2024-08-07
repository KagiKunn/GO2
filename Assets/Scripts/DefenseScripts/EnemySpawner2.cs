using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Unity.Mathematics;

using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class EnemySpawner2 : MonoBehaviour {
	public float minY = -9f;
	public float maxY = 10f;
	private const float fixedX = 20f;
	[SerializeField] public int numberOfObjects = 10;
	private int spawnedEnemy = 0;
	public float maxSpawnInterval = 2f;
	[SerializeField] private int totalWave = 3;
	private int currentWave = 0;
	public ProgressBar progressBar;
	public GameObject stageEndUI;

	void Start() {
		CustomLogger.Log("EnemySpawner2: Start() 호출됨");

		if (progressBar != null) {
			progressBar.SetMaxValue(numberOfObjects * totalWave);
			progressBar.SetValue(0);
		} else {
			CustomLogger.LogWarning("ProgressBar가 설정되지 않았습니다.");
		}

		if (stageEndUI != null) {
			stageEndUI.SetActive(false);
		} else {
			CustomLogger.LogWarning("StageEndUI가 설정되지 않았습니다.");
		}
	}

	public void SpawnWaves(List<GameObject> enemyPrefabs, GameObject bossPrefab) {
		foreach (var obj in enemyPrefabs) {
			Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
		}

		Instantiate(bossPrefab, new Vector3(0, 0, 0), Quaternion.identity);
	}

	// public IEnumerator SpawnWaves(List<GameObject> enemyPrefabs, GameObject bossPrefab) {
	// 	CustomLogger.Log("SpawnWaves() 시작됨");
	//
	// 	while (currentWave < totalWave) {
	// 		currentWave++;
	// 		CustomLogger.Log(currentWave + " 웨이브 시작");
	//
	// 		List<GameObject> wavePrefabs = new List<GameObject>(enemyPrefabs);
	//
	// 		yield return StartCoroutine(SpawnObjects(wavePrefabs));
	//
	// 		spawnedEnemy = 0;
	//
	// 		if (currentWave < totalWave) {
	// 			CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.");
	//
	// 			yield return new WaitForSeconds(5f);
	// 		}
	// 	}
	//
	// 	CustomLogger.Log("보스 소환");
	// 	SpawnBoss(bossPrefab);
	// }

	public void ResetWave() {
		CustomLogger.Log("ResetWave() 호출됨");
		currentWave = 0;
	}

	private IEnumerator SpawnObjects(List<GameObject> wavePrefabs) {
		CustomLogger.Log("SpawnObjects() 시작됨");

		if (wavePrefabs == null || wavePrefabs.Count == 0) {
			CustomLogger.LogError("wavePrefabs가 비어 있습니다. 적 프리팹을 확인하세요.");

			yield break;
		}

		for (int i = 0; i < numberOfObjects; i++) {
			float randomY = Random.Range(minY, maxY);
			GameObject randomPrefab = GetRandomPrefab(wavePrefabs);
			Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
			Instantiate(randomPrefab, spawnPosition, Quaternion.identity, null);

			float waitTime = Random.Range(0, maxSpawnInterval);

			yield return new WaitForSeconds(waitTime);

			spawnedEnemy++;
			CustomLogger.Log("생성한 적의 수 " + spawnedEnemy);

			if (progressBar != null) {
				progressBar.SetValue(spawnedEnemy + (currentWave - 1) * numberOfObjects);
			}

			if (spawnedEnemy >= numberOfObjects) {
				break;
			}
		}
	}

	private void SpawnBoss(GameObject bossPrefab) {
		CustomLogger.Log("SpawnBoss() 호출됨");

		if (bossPrefab != null) {
			Vector3 spawnPosition = new Vector3(fixedX, (minY + maxY) / 2, 0);
			GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity, null);
			boss.GetComponent<Boss>().OnBossDefeated += OnBossDefeated;
		}
	}

	private void OnBossDefeated() {
		CustomLogger.Log("보스가 쓰러졌습니다.");

		if (stageEndUI != null) {
			stageEndUI.SetActive(true);
		}
	}

	private GameObject GetRandomPrefab(List<GameObject> wavePrefabs) {
		float randomValue = Random.Range(0f, 1f);
		float[] percentages = GetPercentages(wavePrefabs.Count);

		for (int i = 0; i < percentages.Length; i++) {
			if (randomValue < percentages[i]) {
				return wavePrefabs[i];
			}

			randomValue -= percentages[i];
		}

		return wavePrefabs[wavePrefabs.Count - 1];
	}

	private float[] GetPercentages(int prefabCount) {
		switch (currentWave) {
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