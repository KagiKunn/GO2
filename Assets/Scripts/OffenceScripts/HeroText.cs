using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class HeroText : MonoBehaviour {
	private Text heroName;
	private Text heroDesc;

	private GameManager gameManager;

	private List<HeroData> selectedHeroes = new List<HeroData>();
	private string filePath;

	private void Awake() {
		GameObject parentObject = this.gameObject;

		List<Transform> activeGrandChildren = new List<Transform>();

		for (int i = 0; i < parentObject.transform.childCount; i++) {
			Transform child = parentObject.transform.GetChild(i);

			if (child.childCount > 1) {
				Transform grandChild = child.GetChild(1);
				activeGrandChildren.Add(grandChild);
			} else {
				CustomLogger.Log("Child " + child.name + " does not have enough children.");
			}
		}

		filePath = Path.Combine(Application.persistentDataPath, "selectedHeroes.json");

		if (File.Exists(filePath)) {
			try {
				string json = File.ReadAllText(filePath);

				HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);

				for (int i = 0; i < wrapper.Heroes.Count && i < activeGrandChildren.Count; i++) {
					HeroData hero = wrapper.Heroes[i];
					Text heroName = activeGrandChildren[i].GetComponent<Text>();

					if (heroName != null) {
						heroName.text = hero.Name;
					} else {
						CustomLogger.Log("No Text component found on grandchild: " + activeGrandChildren[i].name);
					}
				}
			} catch (Exception e) {
				CustomLogger.Log("Error parsing JSON: " + e.Message);
			}
		}
	}
}