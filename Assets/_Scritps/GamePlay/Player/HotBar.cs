using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : Storage
{
    public static HotBar Instance;

    [SerializeField] private Color noneSelectedColor;
    [SerializeField] private Color selectedColor;

    private int slotSelected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override (int, int) ScanSlotToAdd(ItemSO item, int quantity = 1)
    {
        for (int i = 0; i < MAX_SLOT_NUM && quantity > 0; i++)
        {
            if (!DicSlot.ContainsKey(i))
            {
                if (item is ToolSO equipment)
                {
                    DicSlot[i] = new Slot(equipment, quantity);

                    quantity -= quantity;
                }
                else
                {
                    int toAdd = Mathf.Min(item.maxStackSize, quantity);
                    DicSlot[i] = new Slot(item, toAdd);

                    quantity -= toAdd;
                }

                inventorySlots[i].AddItem(item, DicSlot[i].quantity);

                // Khởi tạo model trong hand nếu trùng với slot mà người chơi đang chọn
                if (i == slotSelected)
                    DicSlot[slotSelected].SpawnItem();

                return (quantity, i);
            }
        }

        return (-1, 0);
    }

    public override void AddItemIntoSlot(int slotIndex, ItemSO itemData, int quantity)
    {
        base.AddItemIntoSlot(slotIndex, itemData, quantity);
        if (slotIndex == slotSelected)
        {
            itemData.SpawnItem();
        }
    }

    public override void TrashSlotItem(int slotIndex)
    {
        string itemName = DicSlot[slotIndex].itemSO.itemName;

        if (slotIndex == slotSelected)
        {
            DicSlot[slotSelected].DespawnItem();
            TutorialManager.Instance.HideAllTutorials();
        }
        DicSlot.Remove(slotIndex);

        DicName[itemName].Remove(slotIndex);
    }

    public override void SwapSlotItem(int firstSlot, int lastSlot, bool basic)
    {
        if (firstSlot == lastSlot) return;

        if (firstSlot == slotSelected)
        {
            if (DicSlot.TryGetValue(firstSlot, out Slot slot))
                slot.PutAway();
            if (DicSlot.TryGetValue(lastSlot, out slot))
                slot.SpawnItem();
        }
        else if (lastSlot == slotSelected)
        {
            if (DicSlot.TryGetValue(firstSlot, out Slot slot))
                slot.SpawnItem();
            if (DicSlot.TryGetValue(lastSlot, out slot))
                slot.PutAway();
        }

        base.SwapSlotItem(firstSlot, lastSlot, basic);
    }

    public void SelectingSlot(int slotIndex)
    {
        if (slotIndex == slotSelected) return;

        if (slotIndex < 0)
            slotIndex = inventorySlots.Length-1;
        else if (slotIndex > inventorySlots.Length-1)
            slotIndex = 0;

        inventorySlots[slotSelected].GetComponent<Image>().color = noneSelectedColor;
        inventorySlots[slotIndex].GetComponent<Image>().color = selectedColor;

        if (DicSlot.TryGetValue(slotSelected, out Slot slot))
            slot.PutAway();
        if (DicSlot.TryGetValue(slotIndex, out slot))
            slot.SpawnItem();

        slotSelected = slotIndex;
    }

    public void ChangeOneSlot(bool isIncrease)
    {
        if (isIncrease)
            SelectingSlot(slotSelected + 1);
        else
            SelectingSlot(slotSelected - 1);
    }

    public bool CheckSelectingItemQuantity(int quantity = 1)
    {
        if (DicSlot[slotSelected].quantity >= quantity)
            return true;

        return false;
    }

    public void UseSelectingItem(int quantity = 1)
    {
        DicSlot[slotSelected].UpdatePlayerIndexs();

        if (DicSlot[slotSelected].quantity < quantity)
            return;

        if (DicSlot[slotSelected].quantity > quantity)
            RemoveItemInSlot(slotSelected, quantity);
        else
            RemoveSlotItem(slotSelected);
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

        slotSelected = 1;
        SelectingSlot(0);
    }

    public List<SlotData> GetHotBarData()
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
