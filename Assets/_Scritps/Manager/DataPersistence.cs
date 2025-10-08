using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataPersistence : MonoBehaviour
{
    public static DataPersistence Instance;

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

    // Save and Load All Game Data
    public void SaveGameData()
    {
        GameData gameData = new(GetPlayerData(), GetInventoryData());
        
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(Path, json);
    }

    public void LoadGameData()
    {
        if (File.Exists(Path))
        {
            string json = File.ReadAllText(Path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            PlayerHealth.Instance.Init(gameData.playerData);
            InventoryManager.Instance.Init(gameData.inventoryData);
        }
        else
        {
            PlayerHealth.Instance.Init(new PlayerData());
            InventoryManager.Instance.Init(new List<SlotData>());
        }
    }
}

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public List<SlotData> inventoryData;

    public GameData(PlayerData playerData, List<SlotData> inventoryData)
    {
        this.playerData = playerData;
        this.inventoryData = inventoryData;
    }
}