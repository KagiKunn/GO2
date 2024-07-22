using UnityEngine;

public class Bullet : MonoBehaviour {
	[SerializeField] private float damage;
	[SerializeField] private int penetration;

	public void Initialized(float damage, int penetration) {
		this.damage = damage;
		this.penetration = penetration;
	}

	public float Damage() => damage;
	public int Penetration() => penetration;
}