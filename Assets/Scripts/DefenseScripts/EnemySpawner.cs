using UnityEngine;

using System.Collections;
using System.Collections.Generic;

#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.

public class EnemySpawner : MonoBehaviour {
	//스테이지 종족의 모든 적 병사를 넣는 배열
	[SerializeField] private List<GameObject> allEnemyPrefabs;

	//웨이브에서 사용할 배열
	private List<GameObject> _enemyPrefabs;

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

	void Start() {
		StartCoroutine(SpawnWaves());

		// 웨이브 조정
		IEnumerator SpawnWaves() {
			while (currentWave < totalWave) {
				currentWave++;
				_enemyPrefabs = new List<GameObject>(); //waveごとにList初期化

				switch (currentWave) {
					case 1:
						_enemyPrefabs.Add(allEnemyPrefabs[0]);
						_enemyPrefabs.Add(allEnemyPrefabs[1]);

						break;
					case 2:
						_enemyPrefabs.Add((allEnemyPrefabs[0]));
						_enemyPrefabs.Add((allEnemyPrefabs[1]));
						_enemyPrefabs.Add((allEnemyPrefabs[2]));
						_enemyPrefabs.Add((allEnemyPrefabs[3]));

						break;
					case 3:
						_enemyPrefabs.Add(allEnemyPrefabs[0]);
						_enemyPrefabs.Add(allEnemyPrefabs[1]);
						_enemyPrefabs.Add(allEnemyPrefabs[2]);
						_enemyPrefabs.Add(allEnemyPrefabs[3]);
						_enemyPrefabs.Add(allEnemyPrefabs[4]);

						break;
				}

				CustomLogger.Log(currentWave + "웨이브 시작");

				yield return StartCoroutine(SpawnObjects());

				spawnedEnemy = 0; // 웨이브 종료 시 초기화

				if (currentWave <= totalWave) {
					CustomLogger.Log("웨이브 " + currentWave + " 종료. 다음 웨이브까지 5초 대기.", "yellow");

					yield return new WaitForSeconds(10f);
				}
			}

			CustomLogger.Log("모든 웨이브가 완료되었습니다.", "red");
		}

		IEnumerator SpawnObjects() {
			//오브젝트 최대수 제한까지 반복생성
			for (int i = 0; i < numberOfObjects; i++) {
				// Y축의 랜덤 좌표 생성
				float randomY = Random.Range(minY, maxY);

				// 새로운 오브젝트 생성
				GameObject randomPrefab = GetRandomPrefab();

				Vector3 spawnPosition = new Vector3(fixedX, randomY, 0);
				GameObject enemy = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);

				float waitTime = Random.Range(0, maxSpawnInterval);

				yield return new WaitForSeconds(waitTime);

				spawnedEnemy++;
				CustomLogger.Log("생성한 적의 수 " + spawnedEnemy);

				//최대 생성수에 도달하면 웨이브를 종료
				if (spawnedEnemy == numberOfObjects) {
					break;
				}
			}
		}

		/*確率の区間を０～１までの範囲で分けるイメージ。
		randomValueが任意の数を,0f～1fまでの間で引く
		例えばfloat[] { 0.35f, 0.35f, 0.15f, 0.15f };の場合
		この確率テーブルの配列を巡回しながらその数がどの区間に位置するかを判定する。*/

		GameObject GetRandomPrefab() {
			float randomValue = Random.Range(0f, 1f);
			float[] percentages = GetPercentages();

			for (int i = 0; i < percentages.Length; i++) {
				if (randomValue < percentages[i]) {
					return _enemyPrefabs[i];
				}

				randomValue -= percentages[i];
			}

			return _enemyPrefabs[_enemyPrefabs.Count - 1];
		}

		float[] GetPercentages() {
			switch (currentWave) {
				case 1:
					return new float[] { 0.5f, 0.5f }; //各５０％

					break;
				case 2:
					return new float[] { 0.35f, 0.35f, 0.15f, 0.15f }; //35% 35% 15% 15%

					break;
				case 3:
					return new float[] { 0.25f, 0.25f, 0.2f, 0.2f, 0.1f }; //25% 25% 20% 20% 10%

					break;
				default:
					return new float[] { 1f };
			}
		}
	}
}