using UnityEngine;

public class Character : MonoBehaviour {
	public static float Speed {
		get { return GameManager.Instance.PlayerId == 1 ? 1.2f : 1f; }
	}

	public static float MaxHealth {
		get { return GameManager.Instance.PlayerId == 2 ? 1.2f : 1f; }
	}

	public static float GotDamege {
		get { return GameManager.Instance.PlayerId == 3 ? 0.9f : 1f; }
	}

	public static float Damage {
		get { return GameManager.Instance.PlayerId == 4 ? 1.2f : 1f; }
	}

	public static float WeaponRate {
		get { return GameManager.Instance.PlayerId == 4 ? 0.9f : 1f; }
	}

	public static int Count {
		get { return GameManager.Instance.PlayerId == 5 ? 1 : 0; }
	}
}