using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemInstance> items = new List<ItemInstance>();
    public List<int> slotStates = new List<int>();
    public int additionalSlotCount = 0;
}

