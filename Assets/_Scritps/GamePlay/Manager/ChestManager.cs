using UnityEngine;
using System.Collections.Generic;
using System;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance;

    public InventorySlot[] chestSlots;
    public string currentChestCode;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public string GenerateChestCode()
    {
        return $"chest_{Guid.NewGuid().ToString("N")[..6]}";
    }


    #region [[ --- Inventory Data --- ]]
    // private void AddItemToSlot(int slotIndex, ItemSO item, int quantity)
    // {
    //     if (DicName.TryGetValue(item.itemName, out List<int> indices))
    //         indices.Add(slotIndex);
    //     else
    //         DicName[item.itemName] = new List<int> { slotIndex };

    //     DicSlot[slotIndex] = new Slot(item, quantity);
    //     inventorySlots[slotIndex].AddItem(item, quantity);
    // }

    public void Init(List<ChestData> chestsData)
    {
        foreach (ChestData chestData in chestsData)
        {
            // Gọi Construction Manager Instantiate
            GameObject chestObj = ConstructionManager.Instance.AddPlacedObject(
                                    chestData.objectName, 
                                    chestData.position, 
                                    chestData.rotation);
            Chest chestCpnt = chestObj.GetComponent<Chest>();
            // Set Code cho chest
            chestCpnt.storageCode = chestData.chestCode;
            // Gọi Hàm Init của Chest để fill item vào
            chestCpnt.Init(chestData.items);
        }
    }

    public List<ChestData> GetChestsData()
    {
        List<ChestData> ChestsData = new();

        foreach (var kvp in StorageCodeMap.codeMap)
        {
            if (kvp.Key.StartsWith("chest"))
            {
                Storage storageObj = kvp.Value;
                if (storageObj is Chest chest)
                {
                    ChestsData.Add(new ChestData
                    {
                        objectName = chest.gameObject.name,
                        chestCode = chest.storageCode,
                        position = chest.transform.position,
                        rotation = chest.transform.rotation,
                        items = chest.GetItemsDataInChest()
                    });
                }
            }
        }

        return ChestsData;
    }
    #endregion
}

[Serializable]
public class ChestData
{
    public string objectName;
    public string chestCode;
    public Vector3 position;
    public Quaternion rotation;
    public List<SlotData> items;
}