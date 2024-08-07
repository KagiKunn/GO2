using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Random = System.Random;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class Gacha : MonoBehaviour {
	public static Gacha Instance { get; private set; }
	public List<ItemSO> items;
	private ItemRarity[] itemList;
	private System.Random rand;
	public int normalProbability = 70;
	public int rareProbability = 25;
	public int uniqueProbability = 5;

	public Image resultImage;
	public Text resultText;
	public Text multiGachaResultText;

	private GameObject previousResultsParent;
	public InventoryUI inventoryUI;
	public Text warningText;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		InitializeRandomizer();
		InitializeItems();
		ItemSO GachaItem = GachaRandomItem();
		CustomLogger.Log($"Gacha Item: {GachaItem.itemName}, Rarity: {GachaItem.rarity}");

		if (resultImage != null) {
			resultImage.enabled = false;
		}

		if (resultText != null) {
			resultText.enabled = false;
		}

		if (inventoryUI == null) {
			inventoryUI = FindObjectOfType<InventoryUI>();
		}

		if (warningText != null) {
			warningText.enabled = false;
		}
	}

	private void InitializeItems() {
		itemList = new ItemRarity[normalProbability + rareProbability + uniqueProbability];
		var index = 0;

		// 확률에 맞는 배열 생성
		for (var i = 0; i < normalProbability; i++, index++) {
			itemList[index] = ItemRarity.Normal;
		}

		for (var i = 0; i < rareProbability; i++, index++) {
			itemList[index] = ItemRarity.Rare;
		}

		for (var i = 0; i < uniqueProbability; i++, index++) {
			itemList[index] = ItemRarity.Unique;
		}

		// 배열을 랜덤하게 섞기
		Shuffle(itemList);
	}

	public void Shuffle(ItemRarity[] array) {
		for (var i = array.Length - 1; i > 0; i--) {
			var j = rand.Next(0, i + 1);
			(array[i], array[j]) = (array[j], array[i]);
		}
	}

	private void InitializeRandomizer() {
		if (PlayerDataControl.Instance != null) {
			Guid uuid = Guid.Parse(PlayerDataControl.Instance.UUID);
			byte[] bytes = uuid.ToByteArray();
			ulong seed1 = BitConverter.ToUInt64(bytes, 0);
			ulong seed2 = BitConverter.ToUInt64(bytes, 8);
			rand = new Random((int)(seed1 ^ seed2 ^ (ulong)DateTime.Now.Millisecond));
		} else {
			Guid uuid = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");
			byte[] bytes = uuid.ToByteArray();
			ulong seed1 = BitConverter.ToUInt64(bytes, 0);
			ulong seed2 = BitConverter.ToUInt64(bytes, 8);
			rand = new Random((int)(seed1 ^ seed2 ^ (ulong)DateTime.Now.Millisecond));
		}
	}

	public void OnGachaButtonClicked() {
		if (inventoryUI != null && !inventoryUI.CanAdditems(1)) {
			CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");

			return;
		}

		ItemSO GachaItem = GachaRandomItem();
		DisplayResult(GachaItem);
		AddItemToInventory(GachaItem);
	}

	public void OnMultiGachaButtonClicked(int GachaCount) {
		if (inventoryUI != null && !inventoryUI.CanAdditems(GachaCount)) {
			CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");

			return;
		}

		Dictionary<ItemRarity, int> gachaResults = new Dictionary<ItemRarity, int> {
			{ ItemRarity.Normal, 0 },
			{ ItemRarity.Rare, 0 },
			{ ItemRarity.Unique, 0 }
		};

		List<ItemSO> gachaItems = new List<ItemSO>();

		for (int i = 0; i < GachaCount; i++) {
			ItemSO gachaItem = GachaRandomItem();
			gachaItems.Add(gachaItem);
			gachaResults[gachaItem.rarity]++;
		}

		DisplayMultiGachaResult(gachaResults, GachaCount, gachaItems);

		foreach (var item in gachaItems) {
			AddItemToInventory(item);
		}
	}

	private void AddItemToInventory(ItemSO item) {
		if (inventoryUI != null) {
			inventoryUI.AddItemToInventory(item);
			inventoryUI.UpdateInventoryUI();
		} else {
			CustomLogger.LogWarning("InventoryUI is not assigned.");
		}
	}

	private ItemSO GachaRandomItem() {
		int randomValue = rand.Next(0, itemList.Length);
		ItemRarity selectedRarity = itemList[randomValue];

		List<ItemSO> filteredItems = items.FindAll(item => item.rarity == selectedRarity);
		int randomIndex = rand.Next(0, filteredItems.Count);

		return filteredItems[randomIndex];
	}

	private void DisplayResult(ItemSO item) {
		if (resultImage != null) {
			resultImage.sprite = item.icon;
			resultImage.enabled = true;
		}

		if (resultText != null) {
			resultText.text = $"Item: {item.itemName}\nRarity: {item.rarity}";
			resultText.enabled = true;
		}
	}

	private void DisplayMultiGachaResult(Dictionary<ItemRarity, int> gachaResults, int gachaCount,
	                                     List<ItemSO> gachaItems) {
		foreach (Transform child in multiGachaResultText.transform.parent) {
			if (child.name.StartsWith("GachaItem")) {
				Destroy(child.gameObject);
			}
		}

		if (multiGachaResultText != null) {
			float normalPercent = (float)gachaResults[ItemRarity.Normal] / gachaCount * 100;
			float rarePercent = (float)gachaResults[ItemRarity.Rare] / gachaCount * 100;
			float uniquePercent = (float)gachaResults[ItemRarity.Unique] / gachaCount * 100;

			String resultText = $"Result of {gachaCount} Gachas:\n" +
			                    $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
			                    $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
			                    $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)\n\n" +
			                    "Items:\n";

			foreach (var item in gachaItems) {
				resultText += $"Item: {item.itemName}, Rarity: {item.rarity}\n";
			}

			multiGachaResultText.text = resultText;
			CustomLogger.Log(this.resultText);
		}

		foreach (var item in gachaItems) {
			GameObject parentobject = new GameObject("GachaItem");
			parentobject.transform.SetParent(multiGachaResultText.transform.parent);

			VerticalLayoutGroup vlg = parentobject.AddComponent<VerticalLayoutGroup>();
			vlg.childControlHeight = true;
			vlg.childControlWidth = true;
			vlg.childAlignment = TextAnchor.MiddleCenter;

			GameObject newImage = new GameObject("GachaItem.Image");
			newImage.transform.SetParent(parentobject.transform);
			Image imageComponent = newImage.AddComponent<Image>();
			imageComponent.sprite = item.icon;

			GameObject newText = new GameObject("GachaItemTest");
			newText.transform.SetParent(parentobject.transform);
			Text textComponent = newText.AddComponent<Text>();
			textComponent.text = $"Item: {item.itemName}\nRarity: {item.rarity}";
			textComponent.font = multiGachaResultText.font;
			textComponent.fontSize = multiGachaResultText.fontSize;
			textComponent.color = multiGachaResultText.color;
			textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
			textComponent.verticalOverflow = VerticalWrapMode.Overflow;
		}
	}
}