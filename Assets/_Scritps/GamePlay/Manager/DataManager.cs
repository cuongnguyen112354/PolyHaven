using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [SerializeField] private InitDataSO initDataSO;

    private DataPersistence data = DataPersistence.Instance;
    // C:\Users\Chi Cuong\AppData\LocalLow\DefaultCompany\PolyHaven\data1.json
    private string Path => Application.persistentDataPath + "/data1.json";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Save and Load All Game Data
    public GameData GetGameData()
    {
        return new (
            PlayerHealth.Instance.GetPlayerData(),
            HotBar.Instance.GetHotBarData(),
            Inventory.Instance.GetInventoryData(),
            ChestManager.Instance.GetChestsData(),
            EnvironmentManager.Instance.GetEnvironmentObject(),
            ConstructionManager.Instance.GetConstructionObject()
        );
    }

    public void InitData()
    {
        GameData gameData;

        if (data)
            gameData = data.GetGameData();
        else
        {
            if (File.Exists(Path))
                gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(Path));
            else
                gameData = new GameData(initDataSO);
            CursorManager.Init();    
        }

        if (gameData.playerData != null)
            PlayerHealth.Instance.Init(gameData.playerData);
        if (gameData.hotBarData != null)
            HotBar.Instance.Init(gameData.hotBarData);
        if (gameData.inventoryData != null)
            Inventory.Instance.Init(gameData.inventoryData);
        if (gameData.chestsData != null)
            ChestManager.Instance.Init(gameData.chestsData);
        if (gameData.environmentData != null)
            EnvironmentManager.Instance.Init(gameData.environmentData);
        if (gameData.constructionData != null)
            ConstructionManager.Instance.Init(gameData.constructionData);
        
        if (data)
            Settings.Instance.Init(data.GetSettingsData());
    }
}

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public List<SlotData> hotBarData;
    public List<SlotData> inventoryData;
    public List<ChestData> chestsData;
    public List<EnvironmentObject> environmentData;
    public List<ConstructionObject> constructionData;

    public GameData(
        PlayerData playerData,
        List<SlotData> hotBarData,
        List<SlotData> inventoryData,
        List<ChestData> chestsData,
        List<EnvironmentObject> environmentData,
        List<ConstructionObject> constructionData
    )
    {
        this.playerData = playerData;
        this.hotBarData = hotBarData;
        this.inventoryData = inventoryData;
        this.chestsData = chestsData;
        this.environmentData = environmentData;
        this.constructionData = constructionData;
    }

    public GameData(InitDataSO initDataSO)
    {
        playerData = initDataSO.playerData;
        hotBarData = initDataSO.hotBarData;
        inventoryData = initDataSO.inventoryData;
        chestsData = initDataSO.chestsData;
        environmentData = initDataSO.environmentData;
        constructionData = initDataSO.constructionData;
    }
}