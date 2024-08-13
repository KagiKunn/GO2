using UnityEngine;

public class Result : MonoBehaviour {
	[SerializeField] private GameObject[] titles;

	public void Lose() {
		titles[0].SetActive(true);
		titles[1].SetActive(true);
	}

	public void Win() {
		titles[2].SetActive(true);
		titles[3].SetActive(true);
	}
}