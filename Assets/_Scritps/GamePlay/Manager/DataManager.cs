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

    // Object in Environment Data
    private List<EnvironmentObject> GetEnvironmentData()
    {
        return EnvironmentManager.Instance.GetEnvironmentObject();
    }

    // Construction Data
    private List<ConstructionObject> GetConstructionData()
    {
        return ConstructionManager.Instance.GetConstructionObject();
    }

    // Save and Load All Game Data
    public GameData GetGameData()
    {
        return new (
            GetPlayerData(),
            GetInventoryData(),
            GetEnvironmentData(),
            GetConstructionData()
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
        if (gameData.inventoryData != null)
            InventoryManager.Instance.Init(gameData.inventoryData);
        if (gameData.environmentData != null)
            EnvironmentManager.Instance.Init(gameData.environmentData);
        if (gameData.constructionData != null)
            ConstructionManager.Instance.Init(gameData.constructionData);
        
        if (data)
            Settings.Instance.Init(data.GetSettingsData());
    }
}