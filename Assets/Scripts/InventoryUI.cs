using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    private Canvas canvas;
    public Transform inventoryContent;
    public GameObject itemInfoPanel;
    public TextMeshProUGUI itemInfoText;
    public Button deleteButton;
    public Button closeButton;
    private ItemInstance selectedItem;
    private Transform selectedSlot;
    private const string inventoryFilePath = "inventory.json";

    public InventoryData inventoryData = new InventoryData();
    private Vector2 touchStartPosition;
    private bool isDragging = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        LoadInventory();

        deleteButton.onClick.AddListener(DeleteSelectedItem);
        closeButton.onClick.AddListener(CloseInventory);
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
            if (result.gameObject == deleteButton.gameObject)
            {
                return true;
            }
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


    public void AddItemToInventory(ItemInstance itemInstance)
    {
        CanAddItem();
        inventoryData.items.Add(itemInstance);
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

    public void UpdateSlotUI(Transform slot, ItemInstance itemInstance)
    {
        ItemSlot itemSlotComponent = slot.GetComponent<ItemSlot>();

        if (itemSlotComponent != null)
        {
            itemSlotComponent.ItemInstance = itemInstance;
            
            Image itemImage = slot.Find("ItemImage").GetComponent<Image>();
            TextMeshProUGUI itemName = slot.Find("ItemName").GetComponent<TextMeshProUGUI>();

            if (itemImage == null)
            {
                CustomLogger.LogError("ItemImage component is missing on ItemImage GameObject.");
                return;
            }

            if (itemName == null)
            {
                CustomLogger.LogError("Text component is missing on ItemName GameObject.");
                return;
            }

            if (itemInstance != null)
            {
                itemImage.sprite = itemInstance.itemData.icon;
                itemName.text = itemInstance.itemData.itemName;
                Button slotButton = slot.GetComponent<Button>();

                if (slotButton != null)
                {
                    slotButton.onClick.RemoveAllListeners();
                    slotButton.onClick.AddListener(() => {
                        if (selectedSlot == slot && itemInfoPanel.activeSelf)
                        {
                            itemInfoPanel.SetActive(false);
                        }
                        else
                        {
                            ItemInfo(itemInstance, slot);
                        }
                    });
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
        
        
    }

    // public void SwapItems(ItemSlot slot1, ItemSlot slot2)
    // {
    //     ItemSO tempItem = slot1.item;
    //     slot1.item = slot2.item;
    //     slot2.item = tempItem;
    //     
    //     UpdateInventoryUI();
    //     SaveInventory();
    // }

    public void SaveInventory()
    {
            string jsonContent = JsonUtility.ToJson(inventoryData, true);
            CustomLogger.Log($"JSON Content Before Writing: {jsonContent}");

            JsonInventory.SaveToJson(inventoryData, inventoryFilePath);
            
            CustomLogger.Log($"Saving Inventory: additionalSlotCount = {inventoryData.additionalSlotCount}");
            CustomLogger.Log($"Saving to path: {Path.GetFullPath(inventoryFilePath)}");
            CustomLogger.Log($"JSON Content After Writing: {jsonContent}");
            CustomLogger.Log("저장완료!");
    }

    void LoadInventory()
    {
        inventoryData = JsonInventory.LoadFromJson<InventoryData>(inventoryFilePath) ?? new InventoryData();
        CustomLogger.Log($"Loaded inventoryData from JSON: {JsonUtility.ToJson(inventoryData, true)}");

        if (inventoryData == null)
        {
            CustomLogger.LogError("로드를 실패했습니다.");
            return;
        }
        
        if (inventoryData.items.Count == 0)
        {
            CustomLogger.LogWarning("No items loaded into inventoryData.");
        }
        else
        {
            foreach (var itemInstance in inventoryData.items)
            {
                CustomLogger.Log($"Loading item with ID: {itemInstance.itemID}");
                itemInstance.LoadItemData();
                
                if (itemInstance.itemData == null) 
                {
                    CustomLogger.LogError($"Failed to load item data for item with ID: {itemInstance.itemID}. Possible data corruption.");
                }
            }
            
            for (int i = 0; i < inventoryData.additionalSlotCount; i++)
            {
                GameObject newSlot = Resources.Load<GameObject>("PreFab/Slot");
                if (newSlot != null)
                {
                    Instantiate(newSlot, inventoryContent);
                }
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
        CustomLogger.Log($"Expanding Inventory by {additionalSlots} slots");
        for (int i = 0; i < additionalSlots; i++)
        {
            GameObject newSlot = Resources.Load<GameObject>("PreFab/Slot");
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

    public void ItemInfo(ItemInstance itemInstance, Transform slot)
    {
        selectedItem = itemInstance;
        selectedSlot = slot;

        itemInfoText.text = $"{itemInstance.itemData.itemName}";

        switch (itemInstance.itemData.rarity)
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
        CustomLogger.Log("DeleteSelectedItem() method called.");
        
        if (selectedItem != null && selectedSlot != null)
        {
            CustomLogger.Log($"Attempting to delete item: {selectedItem.itemData.itemName} from slot: {selectedSlot.name}");
            
            if (inventoryData.items.Contains(selectedItem))
            {
                inventoryData.items.Remove(selectedItem);
                CustomLogger.Log($"Item {selectedItem.itemData.itemName} removed from inventory.");
            }
            else
            {
                CustomLogger.LogWarning($"Item {selectedItem.itemData.itemName} not found in inventory.");
                return;
            }
            
            SaveInventory();
            UpdateSlotUI(selectedSlot, null);
            itemInfoPanel.SetActive(false);
            RearrangeInventorySlots();
        }
        else
        {
            CustomLogger.LogWarning("No item selected for deletion or slot is null.");
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

    private void CloseInventory()
    {
        CanvasEnabled();
    }

    
}