using System;

using TMPro;

using UnityEngine;

public class NextEnemy : MonoBehaviour {
	public bool isVisual = false;

	private void Awake() {
		DontDestroyOnLoad(this);

		if (isVisual) { }
	}
}