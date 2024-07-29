using System;
using System.Collections;

using UnityEngine;

public class AchieveManager : MonoBehaviour {
	[SerializeField] private GameObject[] lockCharacter;
	[SerializeField] private GameObject[] unlockCharacter;
	[SerializeField] private GameObject uiNotice;

	private GameManager gameManager;

	enum Achieve {
		UnlockPotato,
		UnlockBean
	}

	private Achieve[] Achieves;

	private WaitForSecondsRealtime waitForSecondsRealtime;

	private void Awake() {
		gameManager = GameManager.Instance;

		Achieves = (Achieve[])Enum.GetValues(typeof(Achieve));

		waitForSecondsRealtime = new WaitForSecondsRealtime(5);

		if (!PlayerPrefs.HasKey("MyData")) {
			Initialized();
		}
	}

	private void Start() {
		UnlockCharacter();
	}

	private void LateUpdate() {
		foreach (Achieve Achieve in Achieves) {
			CheckAchieve(Achieve);
		}
	}

	private void Initialized() {
		PlayerPrefs.SetInt("MyData", 1);

		foreach (Achieve Achieve in Achieves) {
			PlayerPrefs.SetInt(Achieve.ToString(), 0);
		}
	}

	private void UnlockCharacter() {
		for (int i = 0; i < lockCharacter.Length; i++) {
			string AchieveName = Achieves[i].ToString();
			bool isUnlock = PlayerPrefs.GetInt(AchieveName) == 1;

			lockCharacter[i].SetActive(!isUnlock);
			unlockCharacter[i].SetActive(isUnlock);
		}
	}

	private void CheckAchieve(Achieve Achieve) {
		bool isAchieve = false;

		switch (Achieve) {
			case Achieve.UnlockPotato:
				if (gameManager.IsLive) {
					isAchieve = gameManager.Kill >= 10;
				}

				break;

			case Achieve.UnlockBean:
				isAchieve = gameManager.GameTime == gameManager.MaxGameTime;

				break;
		}

		if (isAchieve && PlayerPrefs.GetInt(Achieve.ToString()) == 0) {
			PlayerPrefs.SetInt(Achieve.ToString(), 1);

			for (int i = 0; i < uiNotice.transform.childCount; i++) {
				bool isActive = i == (int)Achieve;

				uiNotice.transform.GetChild(i).gameObject.SetActive(isActive);
			}

			StartCoroutine(NoticeRoutine());
		}
	}

	IEnumerator NoticeRoutine() {
		uiNotice.SetActive(true);

		yield return waitForSecondsRealtime;

		uiNotice.SetActive(false);
	}
}