using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private DataPersistence data = DataPersistence.Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Player Data
    private PlayerData GetPlayerData()
    {
        return PlayerHealth.Instance.GetPlayerData();
    }

    // Inventory Data
    private List<SlotData> GetInventoryData()
    {
        return InventoryManager.Instance.GetInventoryData();
    }

    // Construction Data
    private List<ObjectData> GetConstructionData()
    {
        return ConstructionManager.Instance.GetConstructionData();
    }

    // Save and Load All Game Data
    public GameData GetGameData()
    {
        return new (
            GetPlayerData(),
            GetInventoryData(),
            GetConstructionData()
        );
    }

    public void InitData()
    {
        if (data.gameData.playerData != null)
            PlayerHealth.Instance.Init(data.gameData.playerData);
        if (data.gameData.inventoryData != null)
            InventoryManager.Instance.Init(data.gameData.inventoryData);
        if (data.gameData.constructionData != null)
            ConstructionManager.Instance.Init(data.gameData.constructionData);
        
        if (data)
            Settings.Instance.Init(data.settingsData);
    }
}