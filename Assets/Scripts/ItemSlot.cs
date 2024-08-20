using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public ItemInstance ItemInstance;
	public ItemSO item => ItemInstance?.itemData;
	public Image itemImage;

	private Transform originalParent;
	private Canvas canvas;
	private RectTransform rectTransform;
	private LayoutGroup layoutGroup;
	private int originalSiblingIndex;

	private InventoryUI inventoryUI;
	private ScrollRect scrollRect;

	private bool isDragging = false;

	private void Start() {
		rectTransform = GetComponent<RectTransform>();
		itemImage = GetComponentInChildren<Image>();

		inventoryUI = FindObjectOfType<InventoryUI>();
		scrollRect = GetComponentInParent<ScrollRect>();

		if (ItemInstance == null) {
			CustomLogger.Log($"Slot {gameObject.name} has no ItemInstance assigned.");

			return;
		}

		if (inventoryUI != null) {
			if (ItemInstance != null && item != null) {
				inventoryUI.UpdateSlotUI(this.transform, ItemInstance);
				CustomLogger.Log($"Slot {gameObject.name} initialized with item: {item.itemName}");
			} else {
				CustomLogger.Log($"Slot {gameObject.name} has no item assigned or item data is null.");
			}
		} else {
			CustomLogger.LogError($"InventoryUI not found in the scene for slot {gameObject.name}.");
		}

		if (GetComponentInParent<Canvas>() == null) {
			CustomLogger.Log("Canvas not found in parent hierarchy.");
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		isDragging = true;
		originalParent = transform.parent;
		layoutGroup = originalParent.GetComponent<LayoutGroup>();
		originalSiblingIndex = transform.GetSiblingIndex();

		if (layoutGroup != null) {
			layoutGroup.enabled = false;
		}

		if (scrollRect != null) {
			scrollRect.enabled = false;
		}

		itemImage.raycastTarget = false;
		transform.SetAsLastSibling();

		if (scrollRect != null) {
			scrollRect.vertical = true;

			if (scrollRect.verticalScrollbar != null) {
				scrollRect.verticalScrollbar.gameObject.SetActive(true);
			}
		}
	}

	public void OnDrag(PointerEventData eventData) {
		rectTransform.position = Input.mousePosition;

		if (Input.mouseScrollDelta.y != 0) {
			scrollRect.verticalNormalizedPosition += Input.mouseScrollDelta.y * 0.1f;
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		itemImage.raycastTarget = true;
		isDragging = false;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, results);

		bool itemSwapped = false;

		foreach (var result in results) {
			ItemSlot targetSlot = result.gameObject.GetComponentInParent<ItemSlot>();

			if (targetSlot != null && targetSlot != this) {
				CustomLogger.Log($"RayCast hit: {targetSlot.name}");
				SwapItems(targetSlot);
				itemSwapped = true;

				break;
			}
		}

		ResetSlotPosition();

		if (layoutGroup != null) {
			layoutGroup.enabled = true;
			LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent.GetComponent<RectTransform>());
		}

		if (scrollRect != null) {
			scrollRect.enabled = true;
		}

		if (!itemSwapped) {
			CustomLogger.Log("아이템교환 실패");
		}

		CustomLogger.Log($"OnEndDrag: Parent = {transform.parent.name}, Position = {rectTransform.localPosition}");
	}

	private void SwapItems(ItemSlot targetSlot) {
		if (this.ItemInstance == null || targetSlot.ItemInstance == null) {
			CustomLogger.Log($"Attempted to swap with a null item. This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");

			return;
		}

		CustomLogger.Log($"Before Swap: This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");

		ItemInstance tempInstance = targetSlot.ItemInstance;
		targetSlot.ItemInstance = this.ItemInstance;
		this.ItemInstance = tempInstance;

		CustomLogger.Log($"After Swap: This item: {this.ItemInstance?.itemData.itemName}, Target item: {targetSlot.ItemInstance?.itemData.itemName}");

		if (inventoryUI != null) {
			inventoryUI.UpdateSlotUI(this.transform, this.ItemInstance);
			inventoryUI.UpdateSlotUI(targetSlot.transform, targetSlot.ItemInstance);
		} else {
			CustomLogger.Log("InventoryUI instance not found.");
		}

		targetSlot.rectTransform.anchoredPosition = Vector3.zero;
		rectTransform.anchoredPosition = Vector3.zero;

		CustomLogger.Log($"Swapped items: {this.item?.itemName} with {targetSlot.item?.itemName}");
	}

	private void ResetSlotPosition() {
		transform.SetParent(originalParent, false);
		transform.SetSiblingIndex(originalSiblingIndex);
		rectTransform.anchoredPosition = Vector2.zero;
	}
}