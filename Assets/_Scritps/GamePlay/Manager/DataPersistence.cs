using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataPersistence : MonoBehaviour
{
    public static DataPersistence Instance;

    [SerializeField] private InitDataSO initDataSO;

    // C:\Users\Chi Cuong\AppData\LocalLow\DefaultCompany\PolyHaven\data.json
    private string Path => Application.persistentDataPath + "/data.json";

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
    public void SaveGameData()
    {
        GameData gameData = new (
            GetPlayerData(),
            GetInventoryData(),
            GetConstructionData()
        );

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(Path, json);
    }

    public void LoadGameData()
    {
        GameData gameData;

        if (File.Exists(Path))
        {
            string json = File.ReadAllText(Path);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new (initDataSO);
        }

        if (gameData.playerData != null)
            PlayerHealth.Instance.Init(gameData.playerData);
        if (gameData.inventoryData != null)
            InventoryManager.Instance.Init(gameData.inventoryData);
        if (gameData.constructionData != null)
            ConstructionManager.Instance.Init(gameData.constructionData);
        
        if (GameController.Instance)
            Settings.Instance.Init(GameController.Instance.GetSettingsData());
    }
}

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public List<SlotData> inventoryData;
    public List<ObjectData> constructionData;

    public GameData(
        PlayerData playerData,
        List<SlotData> inventoryData,
        List<ObjectData> constructionData
    )
    {
        this.playerData = playerData;
        this.inventoryData = inventoryData;
        this.constructionData = constructionData;
    }

    public GameData(InitDataSO initDataSO)
    {
        playerData = initDataSO.playerData;
        inventoryData = initDataSO.inventoryData;
        constructionData = initDataSO.constructionData;
    }
}