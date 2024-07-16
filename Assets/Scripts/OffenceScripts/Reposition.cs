using System;

using UnityEngine;

public class Reposition : MonoBehaviour {
	private void OnTriggerExit2D(Collider2D other) {
		if (!other.CompareTag("Area")) return;
		
		
	}
}