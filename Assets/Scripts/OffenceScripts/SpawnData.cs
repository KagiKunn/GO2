using UnityEngine;

[System.Serializable]
public class SpawnData {
	[SerializeField] private float spawnTime;
	[SerializeField] private int spriteType;
	[SerializeField] private int health;
	[SerializeField] private float speed;

	public float SpawnTime => spawnTime;
	public int SpriteType => spriteType;
	public int Health => health;
	public float Speed => speed;
}