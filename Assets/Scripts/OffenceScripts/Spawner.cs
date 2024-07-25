using UnityEngine;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
	private PoolManager poolManager;

	[SerializeField] private SpawnData[] spawnData;

	[SerializeField] private Transform[] spawnPointTransforms;

	private int level;
	private float timer;

	private void Awake() {
		poolManager = GameManager.Instance.PoolManager;
		spawnPointTransforms = GetComponentsInChildren<Transform>();
	}

	private void Update() {
		if(!GameManager.Instance.IsLive) return;
		
		timer += Time.deltaTime;
		level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.GameTime / 10f), spawnData.Length - 1);

		if (timer > spawnData[level].SpawnTime) {
			timer = 0;

			Spawn();
		}
	}

	private void Spawn() {
		GameObject enemy = poolManager.Get(0);

		enemy.transform.position = spawnPointTransforms[Random.Range(1, spawnPointTransforms.Length)].position;
		enemy.GetComponent<Enemy>().Initialized(spawnData[level]);
	}
}