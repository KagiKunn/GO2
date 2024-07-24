using System;

using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	private enum InfoType {
		Exp, Level, Kill, Time, Health
	}

	[SerializeField]
	private InfoType type;

	private Text myText;
	private Slider mySlider;

	private GameManager gameManager;

	private void Awake() {
		myText = GetComponent<Text>();
		mySlider = GetComponent<Slider>();

		gameManager = GameManager.Instance;
	}

	private void LateUpdate() {
		switch (type) {
			case InfoType.Exp:
				float currentExp = gameManager.Exp;
				float maxExp = gameManager.NextExp[gameManager.Level];

				mySlider.value = currentExp / maxExp;

				break;

			case InfoType.Level:
				myText.text = string.Format("Lv.{0:F0}", gameManager.Level);

				break;

			case InfoType.Kill:
				myText.text = string.Format("x{0:F0}", gameManager.Kill);

				break;

			case InfoType.Time:
				float remainTime = gameManager.MaxGameTime - gameManager.GameTime;

				int min = Mathf.FloorToInt(remainTime / 60);
				int sec = Mathf.FloorToInt(remainTime % 60);

				myText.text = string.Format("{0:D2} : {1:D2}", min, sec);

				break;

			case InfoType.Health:
				float currentHealth = gameManager.Health;
				float maxHealth = gameManager.MaxHealth;
				
				mySlider.value = currentHealth / maxHealth;

				break;
		}
	}
}