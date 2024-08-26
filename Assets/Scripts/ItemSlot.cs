using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable CS0618, CS0414 // 형식 또는 멤버는 사용되지 않습니다.

public class ItemSlot : MonoBehaviour {
	public ItemInstance ItemInstance;
	public ItemSO item => ItemInstance?.itemData;
	public Image itemImage;

	private Transform originalParent;
	private Canvas canvas;
	private RectTransform rectTransform;
	private LayoutGroup layoutGroup;
	private int originalSiblingIndex;

	private InventoryUI inventoryUI;

	private bool isDragging = false;

	private void Start() {
		rectTransform = GetComponent<RectTransform>();
		itemImage = GetComponentInChildren<Image>();

		inventoryUI = FindObjectOfType<InventoryUI>();

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
}
//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         isDragging = true;
//         originalParent = transform.parent;
//         layoutGroup = originalParent.GetComponent<LayoutGroup>();
//         originalSiblingIndex = transform.GetSiblingIndex();
//         
//         if (layoutGroup != null)
//         {
//             layoutGroup.enabled = false;
//         }
//
//         itemImage.raycastTarget = false;
//         transform.SetAsLastSibling();
//     }
//
//     public void OnDrag(PointerEventData eventData)
//     {
//         rectTransform.position = Input.mousePosition;
//     }
//
//     public void OnEndDrag(PointerEventData eventData)
//     {
//         itemImage.raycastTarget = true;
//         isDragging = false;
//         
//         List<RaycastResult> results = new List<RaycastResult>();
//         EventSystem.current.RaycastAll(eventData, results);
//
//         bool itemMoved = false;
//         foreach (var result in results)
//         {
//             ItemSlot targetSlot = result.gameObject.GetComponentInParent<ItemSlot>();
//             
//             if (targetSlot != null && targetSlot != this)
//             {
//                 CustomLogger.Log($"RayCast hit: {targetSlot.name}");
//                 SwapItems(targetSlot);
//                 itemMoved = true;
//                 break;
//             }
//         }
//
//         if (!itemMoved)
//         {
//             MoveToEmptySlot();
//         }
//
//         ResetSlotPosition();
//         
//         if (layoutGroup != null)
//         {
//             layoutGroup.enabled = true;
//             LayoutRebuilder.ForceRebuildLayoutImmediate(originalParent.GetComponent<RectTransform>());
//         }
//
//         if (!itemMoved)
//         {
//             CustomLogger.Log("아이템교환 실패");
//         }
//         
//         inventoryUI.SaveInventory();
//         CustomLogger.Log($"OnEndDrag: Parent = {transform.parent.name}, Position = {rectTransform.localPosition}");
//     }
//     
//     private void SwapItems(ItemSlot targetSlot)
//     {
//         if (targetSlot.ItemInstance == null)
//         {
//             targetSlot.ItemInstance = this.ItemInstance;
//             this.ItemInstance = null;
//             
//             inventoryUI.UpdateSlotUI(this.transform, this.ItemInstance);
//             inventoryUI.UpdateSlotUI(targetSlot.transform, targetSlot.ItemInstance);
//         }
//         else
//         {
//             ItemInstance tempInstance = targetSlot.ItemInstance;
//             targetSlot.ItemInstance = this.ItemInstance;
//             this.ItemInstance = tempInstance;
//             
//             inventoryUI.UpdateSlotUI(this.transform, this.ItemInstance);
//             inventoryUI.UpdateSlotUI(targetSlot.transform, targetSlot.ItemInstance);
//         }
//     }
//     
//     private void MoveToEmptySlot()
//     {
//         for (int i = 0; i < inventoryUI.inventoryContent.childCount; i++)
//         {
//             Transform slotTransform = inventoryUI.inventoryContent.GetChild(i);
//             ItemSlot emptySlot = slotTransform.GetComponent<ItemSlot>();
//
//             if (emptySlot != null && emptySlot.ItemInstance == null)
//             {
//                 emptySlot.ItemInstance = this.ItemInstance;
//                 this.ItemInstance = null;
//
//                 inventoryUI.UpdateSlotUI(emptySlot.transform, emptySlot.ItemInstance);
//                 inventoryUI.UpdateSlotUI(this.transform, this.ItemInstance);
//                 
//                 break;
//             }
//         }
//     }
//     
//     private void ResetSlotPosition()
//     {
//         transform.SetParent(originalParent, false);
//         transform.SetSiblingIndex(originalSiblingIndex);
//         rectTransform.anchoredPosition = Vector2.zero;
//     }
// }