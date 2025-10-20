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
        GameData gameData = data.GetGameData();

        if (gameData.playerData != null)
            PlayerHealth.Instance.Init(gameData.playerData);
        if (gameData.inventoryData != null)
            InventoryManager.Instance.Init(gameData.inventoryData);
        if (gameData.constructionData != null)
            ConstructionManager.Instance.Init(gameData.constructionData);
        
        if (data)
            Settings.Instance.Init(data.GetSettingsData());
    }
}