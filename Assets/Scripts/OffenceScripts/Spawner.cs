using System;

using UnityEngine;

using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
	private PoolManager poolManager;

	[SerializeField]
	private Transform[] spawnPointTransforms;

	private float timer;

	private void Awake() {
		poolManager = GameManager.Instance.PoolManager;
		spawnPointTransforms = GetComponentsInChildren<Transform>();
	}

	private void Update() {
		timer += Time.deltaTime;

		if (timer > 0.2f) {
			timer = 0;

			Spawn();
		}
	}

	private void Spawn() {
		GameObject enemy = poolManager.Get(Random.Range(0, 2));

		enemy.transform.position = spawnPointTransforms[Random.Range(1, spawnPointTransforms.Length)].position;
	}
}