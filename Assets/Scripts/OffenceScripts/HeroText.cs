using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class HeroText : MonoBehaviour {
	private GameManager gameManager;

	private List<HeroData> selectedHeroes = new List<HeroData>();
	private string filePath;

	private void Awake() {
		GameObject charcterGroup = this.gameObject;

		List<Transform> activeHeroColor = new List<Transform>();
		List<Transform> activeHeroIcon = new List<Transform>();
		List<Transform> activeTextName = new List<Transform>();
		List<Transform> activeTextAdvantage = new List<Transform>();

		for (int i = 0; i < charcterGroup.transform.childCount; i++) {
			Transform eachCharcter = charcterGroup.transform.GetChild(i);

			if (eachCharcter.childCount > 1) {
				Transform heroIcon = eachCharcter.GetChild(0);
				Transform textName = eachCharcter.GetChild(1);
				Transform textAdvantage = eachCharcter.GetChild(2);

				activeHeroColor.Add(eachCharcter);
				activeHeroIcon.Add(heroIcon);
				activeTextName.Add(textName);
				activeTextAdvantage.Add(textAdvantage);
			} else {
				CustomLogger.Log("Child " + eachCharcter.name + " does not have enough children.");
			}
		}

			try {
				HeroList[] heroList = PlayerLocalManager.Instance.lHeroeList;

				for (int i = 0; i < heroList.Length && i < activeTextName.Count; i++) {
					HeroData hero = HeroManager.Instance.heroDataList.Find(h=>h.Name == heroList[i].Item1);

					Image heroColor = activeHeroColor[i].GetComponent<Image>();
					Image heroIcon = activeHeroIcon[i].GetComponent<Image>();
					Text heroName = activeTextName[i].GetComponent<Text>();
					Text heroAdvantage = activeTextAdvantage[i].GetComponent<Text>();

					if (heroName != null) {
						heroColor.color = new Color(hero.r / 255f, hero.g / 255f, hero.b / 255f);
						heroIcon.sprite = hero.ProfileImg;
						heroName.text = hero.Name;
						heroAdvantage.text = hero.OffenceHeroAdvantage;
					} else {
						CustomLogger.Log("No Text component found on grandchild: " + activeTextName[i].name);
					}
				}
			} catch (Exception e) {
				CustomLogger.Log("Error parsing JSON: " + e.Message);
			}
	}
}