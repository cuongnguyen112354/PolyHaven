using UnityEngine;
using System.Collections.Generic;

public class Inventory : Storage
{
    public static Inventory Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    #region [[ --- Inventory Data --- ]]
    private void AddItemToSlot(int slotIndex, ItemSO item, int quantity)
    {
        if (DicName.TryGetValue(item.itemName, out List<int> indices))
            indices.Add(slotIndex);
        else
            DicName[item.itemName] = new List<int> { slotIndex };

        DicSlot[slotIndex] = new Slot(item, quantity);
        inventorySlots[slotIndex].AddItem(item, quantity);
    }

    public void Init(List<SlotData> slotDatas)
    {
        List<string> itemExist = new();

        foreach (SlotData slotData in slotDatas)
        {
            if (!itemExist.Contains(slotData.itemName))
                itemExist.Add(slotData.itemName);

            ItemSO item = Resources.Load<ItemSO>($"_ItemSO/{slotData.typeItem}/{slotData.itemName}");

            AddItemToSlot(slotData.slotIndex, item, slotData.quantity);
        }
    }

    public List<SlotData> GetInventoryData()
    {
        List<SlotData> slotDatas = new();

        foreach (KeyValuePair<string, List<int>> entry in DicName)
        {
            foreach (int index in entry.Value)
            {
                if (DicSlot.TryGetValue(index, out Slot slot))
                {
                    slotDatas.Add(new SlotData
                    {
                        slotIndex = index,
                        itemName = entry.Key,
                        typeItem = slot.itemSO.GetType().Name,
                        quantity = slot.quantity
                    });
                }
            }
        }

        return slotDatas;
    }
    #endregion
}