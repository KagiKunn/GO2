using System;
using System.Collections.Generic;
using System.Globalization;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class InternalInit : MonoBehaviour {
	private GameObject current;
	private GameObject max;

	private GameObject gold;
	private GameObject soul;

	void Start() {
		current = GameObject.Find("Current");
		max = GameObject.Find("Max");
		gold = GameObject.Find("GoldInput");
		soul = GameObject.Find("SoulInput");

		current.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleHp.ToString(CultureInfo.CurrentCulture);
		max.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleMaxHp.ToString(CultureInfo.CurrentCulture);
		gold.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lMoney.ToString();
		soul.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lPoint.ToString();

		List<HeroData> heroList = HeroManager.Instance.selectedHeroes;

		for (int i = 0; i < heroList.Count; i++) {
			HeroData hero = heroList[i];
			CustomLogger.Log(Resources.Load<GameObject>("Image/" + hero.Name), Color.cyan);
			Transform heroCell = GameObject.Find("Heroes").transform.GetChild(i);
			GameObject heroObject = Instantiate(Resources.Load<GameObject>("Image/" + hero.Name), heroCell);
			heroObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			heroObject.transform.localPosition = Vector3.zero;

			if (hero != null) {
				CustomLogger.Log(hero.Name);
			}
		}
	}

	private void Update() {
		current.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleHp.ToString(CultureInfo.CurrentCulture);
		gold.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lMoney.ToString();
	}
}