using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UnityEditor;

public class InventoryUI : MonoBehaviour {
	private Canvas canvas;
	private ScrollRect scrollRect;
	public Transform inventoryContent;
	private const string inventoryFilePath = "inventory.json";

	public InventoryData inventoryData = new InventoryData();

	private void Start() {
		canvas = GetComponent<Canvas>();
		scrollRect = GetComponentInChildren<ScrollRect>();
		LoadInventory();
	}

	public void CanvasEnabled() {
		canvas.enabled = !canvas.enabled;

		if (canvas.enabled) {
			ResetScrollRect();
			UpdateInventoryUI();
		}
	}

	private void ResetScrollRect() {
		scrollRect.verticalNormalizedPosition = 1f;
	}

	public void AddItemToInventory(ItemSO item) {
		if (CanAddItem()) {
			inventoryData.items.Add(item);
			SaveInventory();
			UpdateInventoryUI();
		} else {
			CustomLogger.LogWarning("인벤토리에 더 이상 아이템을 추가할 수 없습니다.");
		}
	}

	public bool CanAddItem() {
		return inventoryData.items.Count < inventoryContent.childCount;
	}

	public void UpdateInventoryUI() {
		for (int i = 0; i < inventoryData.items.Count; i++) {
			Transform slot = inventoryContent.GetChild(i);
			UpdateSlotUI(slot, inventoryData.items[i]);
		}

		for (int i = inventoryData.items.Count; i < inventoryContent.childCount; i++) {
			Transform slot = inventoryContent.GetChild(i);
			UpdateSlotUI(slot, null);
		}
	}

	private void UpdateSlotUI(Transform slot, ItemSO item) {
		Image itemImage = slot.Find("ItemImage").GetComponent<Image>();
		TextMeshProUGUI itemName = slot.Find("ItemName").GetComponent<TextMeshProUGUI>();

		if (itemImage == null) {
			CustomLogger.LogError("ItemImage component is missing on ItemImage GameObject.");
		}

		if (itemName == null) {
			CustomLogger.LogError("Text component is missing on ItemName GameObject.");
		}

		if (item != null) {
			itemImage.sprite = item.icon;
			itemName.text = item.itemName;
		} else {
			itemImage.sprite = null;
			itemName.text = "";
		}
	}

	void SaveInventory() {
		JsonInventory.SaveToJson(inventoryData, inventoryFilePath);
	}

	void LoadInventory() {
		inventoryData = JsonInventory.LoadFromJson<InventoryData>(inventoryFilePath) ?? new InventoryData();

		int currentSlotCount = inventoryContent.childCount;

		for (int i = 0; i < inventoryData.additionalSlotCount; i++) {
			GameObject newSlot =
				(GameObject)AssetDatabase.LoadAssetAtPath("Assets/JSFolder/PreFab/Slot.prefab", typeof(GameObject));

			if (newSlot != null) {
				Instantiate(newSlot, inventoryContent);
			}
		}

		UpdateInventoryUI();
	}

	public void ClearInventory() {
		inventoryData.items.Clear();

		int baseSlotCount = inventoryContent.childCount - inventoryData.additionalSlotCount;

		for (int i = inventoryContent.childCount - 1; i >= baseSlotCount; i--) {
			Destroy(inventoryContent.GetChild(i).gameObject);
		}

		inventoryData.additionalSlotCount = 0;
		SaveInventory();
		UpdateInventoryUI();
	}

	public void ExpandInventory(int additionalSlots) {
		for (int i = 0; i < additionalSlots; i++) {
			GameObject newSlot =
				(GameObject)AssetDatabase.LoadAssetAtPath("Assets/JSFolder/PreFab/Slot.prefab", typeof(GameObject));

			if (newSlot != null) {
				Instantiate(newSlot, inventoryContent);
			}
		}

		inventoryData.additionalSlotCount += additionalSlots;
		SaveInventory();
		UpdateInventoryUI();
	}

	public void OnExpandButtonClicked() {
		ExpandInventory(4);
	}
}