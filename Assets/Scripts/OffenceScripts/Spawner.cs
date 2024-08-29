using UnityEngine;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
	private PoolManager poolManager;

	[SerializeField] private SpawnData[] spawnData;

	[SerializeField] private Transform[] spawnPointTransforms;
	[SerializeField] private float levelTime;

	private int level;
	private float timer;
	private float spawnTime;

	private GameManager gameManager;

	private void Awake() {
		gameManager = GameManager.Instance;

		poolManager = GameManager.Instance.PoolManager;
		spawnPointTransforms = GetComponentsInChildren<Transform>();
		levelTime = gameManager.MaxGameTime / spawnData.Length;

		spawnTime = spawnData[Random.Range(0, spawnData.Length)].SpawnTime - ((PlayerLocalManager.Instance.lStage - 1) * 0.1f);
	}

	private void Update() {
		if (!gameManager.IsLive) return;

		timer += Time.deltaTime;
		level = Mathf.Min(Mathf.FloorToInt(gameManager.GameTime / levelTime), spawnData.Length - 1);

		if (timer > spawnTime) {
			timer = 0;

			Spawn();
		}
	}

	private void Spawn() {
		GameObject enemy = poolManager.GetObject(PlayerLocalManager.Instance.lSelectedRace, Random.Range(0, poolManager.DarkElfPrefabs.Length));

		enemy.transform.position = spawnPointTransforms[Random.Range(1, spawnPointTransforms.Length)].position;

		Transform child = enemy.transform.GetChild(0);

		child.GetComponent<Enemy>().Initialized(spawnData[Random.Range(0, spawnData.Length)]);
	}
}