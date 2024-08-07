using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;


public class InventoryUI : MonoBehaviour
{
    private Canvas canvas;
    private ScrollRect scrollRect;
    public Transform inventoryContent;
    public GameObject itemInfoPanel;
    public TextMeshProUGUI itemInfoText;
    public Button deleteButton;
    private ItemSO selectedItem;
    private Transform selectedSlot;
    private const string inventoryFilePath = "inventory.json";

    public InventoryData inventoryData = new InventoryData();

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        scrollRect = GetComponentInChildren<ScrollRect>();
        LoadInventory();

        deleteButton.onClick.AddListener(DeleteSelectedItem);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!InventorySlotPointer())
            {
                itemInfoPanel.SetActive(false);
            }
        }
    }

    private bool InventorySlotPointer()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(inventoryContent))
            {
                return true;
            }
        }
        return false;
    }

    public void CanvasEnabled()
    {
        canvas.enabled = !canvas.enabled;
        if (canvas.enabled)
        {
            ResetScrollRect();
            UpdateInventoryUI();
        }
        else
        {
            if (itemInfoPanel != null)
            {
                itemInfoPanel.SetActive(false);
            }
        }
    }

    private void ResetScrollRect()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void AddItemToInventory(ItemSO item)
    {
        CanAddItem();
        inventoryData.items.Add(item);
        SaveInventory();
        UpdateInventoryUI();
    }

    public bool CanAddItem()
    {
        return inventoryData.items.Count < inventoryContent.childCount;
    }

    public bool CanAdditems(int itemCount)
    {
        return inventoryData.items.Count + itemCount <= inventoryContent.childCount;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventoryData.items.Count; i++)
        {
            Transform slot = inventoryContent.GetChild(i);
            UpdateSlotUI(slot, inventoryData.items[i]);
        }

        for (int i = inventoryData.items.Count; i < inventoryContent.childCount; i++)
        {
            Transform slot = inventoryContent.GetChild(i);
            UpdateSlotUI(slot, null);
        }
    }

    private void UpdateSlotUI(Transform slot, ItemSO item)
    {
        Image itemImage = slot.Find("ItemImage").GetComponent<Image>();
        TextMeshProUGUI itemName = slot.Find("ItemName").GetComponent<TextMeshProUGUI>();

        if (itemImage == null)
        {
            CustomLogger.LogError("ItemImage component is missing on ItemImage GameObject.");
        }

        if (itemName == null)
        {
            CustomLogger.LogError("Text component is missing on ItemName GameObject.");
        }

        if (item != null)
        {
            itemImage.sprite = item.icon;
            itemName.text = item.itemName;
            Button sloButton = slot.GetComponent<Button>();

            if (sloButton != null)
            {
                sloButton.onClick.RemoveAllListeners();
                sloButton.onClick.AddListener(() => ItemInfo(item, slot));
            }
        }
        else
        {
            itemImage.sprite = null;
            itemName.text = "";
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(()=>{itemInfoPanel.SetActive(false);});
            }
        }
    }

    void SaveInventory()
    {
        JsonInventory.SaveToJson(inventoryData, inventoryFilePath);
    }

    void LoadInventory()
    {
        inventoryData = JsonInventory.LoadFromJson<InventoryData>(inventoryFilePath) ?? new InventoryData();

        int currentSlotCount = inventoryContent.childCount;

        for (int i = 0; i < inventoryData.additionalSlotCount; i++)
        {
            GameObject newSlot =
                (GameObject)AssetDatabase.LoadAssetAtPath("Assets/JSFolder/PreFab/Slot.prefab", typeof(GameObject));
            if (newSlot != null)
            {
                Instantiate(newSlot, inventoryContent);
            }
        }

        UpdateInventoryUI();
    }

    public void ClearInventory()
    {
        inventoryData.items.Clear();

        int baseSlotCount = inventoryContent.childCount - inventoryData.additionalSlotCount;
        for (int i = inventoryContent.childCount - 1; i >= baseSlotCount; i--)
        {
            Destroy(inventoryContent.GetChild(i).gameObject);
        }

        inventoryData.additionalSlotCount = 0;
        SaveInventory();
        UpdateInventoryUI();
    }

    public void ExpandInventory(int additionalSlots)
    {
        for (int i = 0; i < additionalSlots; i++)
        {
            GameObject newSlot =
                (GameObject)AssetDatabase.LoadAssetAtPath("Assets/JSFolder/PreFab/Slot.prefab", typeof(GameObject));
            if (newSlot != null)
            {
                Instantiate(newSlot, inventoryContent);
            }
        }

        inventoryData.additionalSlotCount += additionalSlots;
        SaveInventory();
        UpdateInventoryUI();
    }

    public void OnExpandButtonClicked()
    {
        ExpandInventory(4);
    }

    public void ItemInfo(ItemSO item, Transform slot)
    {
        selectedItem = item;
        selectedSlot = slot;

        itemInfoText.text = $"{item.itemName}";

        switch (item.rarity)
        {
            case ItemRarity.Normal:
                itemInfoText.color = Color.white;
                break;
            case ItemRarity.Rare:
                itemInfoText.color = Color.blue;
                break;
            case ItemRarity.Unique:
                itemInfoText.color = Color.red;
                break;
            default:
                itemInfoText.color = Color.white;
                break;
        }

        RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
        RectTransform panelRectTransform = itemInfoPanel.GetComponent<RectTransform>();

        Vector3 worldPosition = slotRectTransform.TransformPoint(new Vector3(slotRectTransform.rect.width, 0, 0));
        panelRectTransform.position = worldPosition;
        panelRectTransform.localScale = new Vector3(0.25f, 0.5f, 1f);
        
        itemInfoPanel.SetActive(true);
    }

    public void DeleteSelectedItem()
    {
        if (selectedItem != null && selectedSlot != null)
        {
            inventoryData.items.Remove(selectedItem);
            SaveInventory();
            UpdateSlotUI(selectedSlot, null);
            itemInfoPanel.SetActive(false);
            RearrangeInventorySlots();
        }
    }

    private void RearrangeInventorySlots()
    {
        for (int i = 0; i < inventoryData.items.Count; i++)
        {
            Transform slot = inventoryContent.GetChild(i);
            UpdateSlotUI(slot, inventoryData.items[i]);
        }

        for (int i = inventoryData.items.Count; i < inventoryContent.childCount; i++)
        {
            Transform slot = inventoryContent.GetChild(i);
            UpdateSlotUI(slot, null);
        }
    }
}