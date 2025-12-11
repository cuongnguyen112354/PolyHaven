using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Chest : Storage, IInteractable
{
    [SerializeField] private ItemSO itemData;

    private Animator animator;
    private bool isOpen;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        isOpen = false;
    }

    private void OpenChest()
    {
        if (ChestManager.Instance.currentChestCode != storageCode)
            InitChestUI();

        GameManager.Instance.ActiveUI("Chest");

        isOpen = true;
        animator.SetBool("isOpen", isOpen);
    }

    private void CloseChest()
    {
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
    }

    private void FillItemIntoUI()
    {
        for (int slotIndex=0; slotIndex<inventorySlots.Length; slotIndex++)
        {
            inventorySlots[slotIndex].slotIndex = slotIndex;
            inventorySlots[slotIndex].storageCode = storageCode;

            // Nếu có Item ở Slot thì sẽ làm rỗng
            if (inventorySlots[slotIndex].transform.childCount != 1 &&
                inventorySlots[slotIndex].transform.childCount == 2)
            {
                inventorySlots[slotIndex].RemoveItem();
            }

            if (DicSlot.ContainsKey(slotIndex))
                inventorySlots[slotIndex].AddItem(DicSlot[slotIndex].itemSO, DicSlot[slotIndex].quantity);
        }
    }

    public (Sprite, string) HowInteract()
    {
        return (itemData.targetIcon, itemData.textTutorial);
    }

    public string GetItemName()
    {
        return gameObject.name;
    }

    public void Affected(int damage = 0)
    {
        if (isOpen)
            CloseChest();
        else
            OpenChest();
    }

    // public Dictionary<string, List<int>> GetDicName()
    // {
    //     return DicName;
    // }

    public void InitChestUI()
    {
        inventorySlots = ChestManager.Instance.chestSlots;
        ChestManager.Instance.currentChestCode = storageCode;
        FillItemIntoUI();
    }

    private void AddItemToSlot(int slotIndex, ItemSO item, int quantity)
    {
        if (DicName.TryGetValue(item.itemName, out List<int> indices))
            indices.Add(slotIndex);
        else
            DicName[item.itemName] = new List<int> { slotIndex };

        DicSlot[slotIndex] = new Slot(item, quantity);
        // inventorySlots[slotIndex].AddItem(item, quantity);
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

    public List<SlotData> GetItemsDataInChest()
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
}