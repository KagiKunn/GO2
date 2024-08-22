using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemInstance> items = new List<ItemInstance>();
    public int additionalSlotCount = 0;
}

