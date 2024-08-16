using System;

using UnityEngine;

public class Follow : MonoBehaviour {
	private RectTransform rectTransform;

	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}

	private void FixedUpdate() {
		rectTransform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.Player[GameManager.Instance.PlayerId].transform.position);
	}
}