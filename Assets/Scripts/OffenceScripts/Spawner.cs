using UnityEngine;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
	private PoolManager poolManager;

	[SerializeField] private SpawnData[] spawnData;

	[SerializeField] private Transform[] spawnPointTransforms;
	[SerializeField] private float levelTime;

	private int level;
	private float timer;
	private GameManager gameManager;

	private void Awake() {
		gameManager = GameManager.Instance;

		poolManager = GameManager.Instance.PoolManager;
		spawnPointTransforms = GetComponentsInChildren<Transform>();
		levelTime = gameManager.MaxGameTime / spawnData.Length;
	}

	private void Update() {
		if (!gameManager.IsLive) return;

		timer += Time.deltaTime;
		level = Mathf.Min(Mathf.FloorToInt(gameManager.GameTime / levelTime), spawnData.Length - 1);

		if (timer > spawnData[level].SpawnTime) {
			timer = 0;

			Spawn();
		}
	}

	private void Spawn() {
		GameObject enemy = poolManager.Get(0);

		enemy.transform.position = spawnPointTransforms[Random.Range(1, spawnPointTransforms.Length)].position;

		Transform child = enemy.transform.GetChild(0);

		child.GetComponent<Enemy>().Initialized(spawnData[0]);
	}
}