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
				CustomLogger.Log("Grandchild Name: " + grandChild.name);
			} else {
				CustomLogger.Log("Child " + child.name + " does not have enough children.");
			}
		}

		filePath = Path.Combine(Application.persistentDataPath, "selectedHeroes.json");

		CustomLogger.Log("File path: " + filePath);

		if (File.Exists(filePath)) {
			try {
				CustomLogger.Log("File exists: " + filePath);

				string json = File.ReadAllText(filePath);
				CustomLogger.Log("JSON content: " + json);

				HeroDataWrapper wrapper = JsonUtility.FromJson<HeroDataWrapper>(json);
				CustomLogger.Log("Number of heroes in JSON: " + wrapper.Heroes.Count);
				CustomLogger.Log("Number of Active Grand Children: " + activeGrandChildren.Count);

				for (int i = 0; i < wrapper.Heroes.Count && i < activeGrandChildren.Count; i++) {
					HeroData hero = wrapper.Heroes[i];
					Text heroName = activeGrandChildren[i].GetComponent<Text>();

					if (heroName != null) {
						heroName.text = hero.Name;
						CustomLogger.Log("Setting text of grandchild: " + hero.Name);
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