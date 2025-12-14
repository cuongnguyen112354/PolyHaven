using UnityEngine;
using System.Collections.Generic;

public class Storage : MonoBehaviour
{
    protected readonly Dictionary<string, List<int>> DicName = new();
    protected readonly Dictionary<int, Slot> DicSlot = new();

    [SerializeField] protected InventorySlot[] inventorySlots;

    public string storageCode;

    protected int MAX_SLOT_NUM;

    protected virtual void Start()
    {     
        MAX_SLOT_NUM = inventorySlots.Length;

        for (int i = 0; i < MAX_SLOT_NUM; i++)
        {
            inventorySlots[i].slotIndex = i;
            inventorySlots[i].storageCode = storageCode;
        }

        StorageCodeMap.AddCode(storageCode, this);
    }

    protected void RemoveItemInSlot(int slotIndex, int quantity)
    {
        DicSlot[slotIndex].quantity -= quantity;
        inventorySlots[slotIndex].UpdateQuantityUI(DicSlot[slotIndex].quantity);
    }

    public void RemoveSlotItem(int slotIndex)
    {
        TrashSlotItem(slotIndex);
        inventorySlots[slotIndex].RemoveItem();
    }

    // Nếu add thành công thì sẽ trả về (tín hiệu, slotIndex, số còn dư)
    // tín hiệu -1: Add thất bại
    // tín hiệu 0: Không còn dư
    // tín hiệu > 0: Số còn dư
    protected virtual (int, int) ScanSlotToAdd(ItemSO item, int quantity = 1)
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

                return (quantity, i);
            }
        }

        return (-1, 0);
    }

    private void RemoveItemFromSlots(List<int> slotIndexs, int quantity)
    {
        int remaining = quantity;

        for (int i = slotIndexs.Count - 1; i >= 0; i--)
        {
            if (DicSlot[slotIndexs[i]].quantity > remaining)
            {
                RemoveItemInSlot(slotIndexs[i], remaining);
                return;
            }

            if (DicSlot[slotIndexs[i]].quantity < remaining)
            {
                remaining -= DicSlot[slotIndexs[i]].quantity;
            }

            RemoveSlotItem(slotIndexs[i]);
        }
    }

    public int ScanSlotsToGetQuantity(List<int> slots)
    {
        int quantity = 0;

        foreach (int slot in slots)
        {
            quantity += DicSlot[slot].quantity;
        }

        return quantity;
    }

    public (ItemSO, int) GetSlotIndexInfor(int slotIndex)
    {
        return (DicSlot[slotIndex].itemSO, DicSlot[slotIndex].quantity);
    }

    public virtual void TrashSlotItem(int slotIndex)
    {
        string itemName = DicSlot[slotIndex].itemSO.itemName;
        DicSlot.Remove(slotIndex);

        DicName[itemName].Remove(slotIndex);
    }

    public bool IsAddItem(ItemSO itemData, int quantity = 1)
    {
        int qty = quantity;

        if (DicName.TryGetValue(itemData.itemName, out List<int> indices))
        {
            foreach (int index in indices)
            {
                Slot slot = DicSlot[index];
                int toAdd = Mathf.Min(slot.SpaceLeft, quantity);
                slot.quantity += toAdd;
                quantity -= toAdd;

                inventorySlots[index].UpdateQuantityUI(DicSlot[index].quantity);

                if (quantity <= 0)
                {
                    UIManager.Instance.ShowPickupNotify(qty, itemData.itemName);
                    return true;
                }
            }
        }

        (int result, int slotIndex) = ScanSlotToAdd(itemData, quantity);

        if (result == -1)
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        else
        {
            while (result != -1)
            {
                if (DicName.TryGetValue(itemData.itemName, out List<int> indexs))
                    indexs.Add(slotIndex);
                else
                    DicName[itemData.itemName] = new List<int> { slotIndex };

                if (result == 0)
                {
                    UIManager.Instance.ShowPickupNotify(qty, itemData.itemName);
                    return true;
                }

                (result, slotIndex) = ScanSlotToAdd(itemData, quantity);
            }
        }

        return false;
    }

    public virtual void AddItemIntoSlot(int slotIndex, ItemSO itemData, int quantity)
    {
        // Trường hợp Add
        if (!DicSlot.ContainsKey(slotIndex))
        {
            if (!DicName.ContainsKey(itemData.itemName))
                DicName[itemData.itemName] = new List<int> { slotIndex };
            else
                DicName[itemData.itemName].Add(slotIndex);
            
            Slot slot = new (itemData, quantity);
            DicSlot.Add(slotIndex, slot);

            inventorySlots[slotIndex].UpdateUI(quantity);
        }
        // Trường hợp Merge
        else
        {
            int qty = quantity;

            Slot slot = DicSlot[slotIndex];
            int toAdd = Mathf.Min(slot.SpaceLeft, quantity);
            slot.quantity += toAdd;
            quantity -= toAdd;

            inventorySlots[slotIndex].UpdateQuantityUI(DicSlot[slotIndex].quantity);

            if (quantity > 0)
                IsAddItem(itemData, quantity);
        }
    }

    public int GetQuantityItem(string itemName)
    {
        if (DicName.TryGetValue(itemName, out List<int> slots))
            return ScanSlotsToGetQuantity(slots);

        return 0;
    }

    public void RemoveItem(string itemName, int quantity = 1)
    {
        if (DicName.TryGetValue(itemName, out List<int> slotIndexs))
            RemoveItemFromSlots(slotIndexs, quantity);
    }

    // public void RemoveItems(List<Ingredient> ingredients)
    // {
    //     foreach (Ingredient ingredient in ingredients)
    //     {
    //         if (DicName.TryGetValue(ingredient.item.itemName, out List<int> slotIndexs))
    //             RemoveItemFromSlots(slotIndexs, ingredient.quantity);
    //     }
    // }

    public virtual void SwapSlotItem(int firstSlot, int lastSlot, bool basic)
    {
        if (firstSlot == lastSlot) return;

        if (basic)
        {
            if (DicName.TryGetValue(DicSlot[firstSlot].itemSO.itemName, out List<int> slots))
                slots[slots.IndexOf(firstSlot)] = lastSlot;

            DicSlot[lastSlot] = DicSlot[firstSlot];
            DicSlot.Remove(firstSlot);

            inventorySlots[firstSlot].UpdateUI(0);
            inventorySlots[lastSlot].UpdateUI(DicSlot[lastSlot].quantity);
        }
        else
        {
            if (DicName.TryGetValue(DicSlot[firstSlot].itemSO.itemName, out List<int> fslots))
                fslots[fslots.IndexOf(firstSlot)] = lastSlot;
            if (DicName.TryGetValue(DicSlot[lastSlot].itemSO.itemName, out List<int> lslots))
                lslots[lslots.IndexOf(lastSlot)] = firstSlot;

            Slot slot = DicSlot[firstSlot];
            DicSlot[firstSlot] = DicSlot[lastSlot];
            DicSlot[lastSlot] = slot;

            inventorySlots[firstSlot].UpdateUI(DicSlot[firstSlot].quantity);
            inventorySlots[lastSlot].UpdateUI(DicSlot[lastSlot].quantity);
        }
    }
}

public class Slot
{
    public ItemSO itemSO;
    public int quantity;

    public int SpaceLeft => itemSO != null ? itemSO.maxStackSize - quantity : 0;

    public Slot(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
    }

    public void SpawnItem()
    {
        itemSO.SpawnItem();
    }

    public void UpdatePlayerIndexs()
    {
        if (itemSO is FoodSO foodSO)
            foodSO.UpdatePlayerIndexs();
    }

    public void DespawnItem()
    {
        itemSO.DespawnItem();
    }

    public void PutAway()
    {
        itemSO.PutAway();
    }
}

[System.Serializable]
public class SlotData
{
    public int slotIndex;
    public string itemName;
    public string typeItem;
    public int quantity;
}