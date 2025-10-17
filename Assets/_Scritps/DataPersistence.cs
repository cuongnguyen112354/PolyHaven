using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class DataPersistence : MonoBehaviour
{
    public static DataPersistence Instance;
    [HideInInspector] public SettingsData settingsData;
    [HideInInspector] public GameData gameData;

    public Versions selectedVersion;

    [SerializeField] private InitDataSO initDataSO;

    // C:\Users\Chi Cuong\AppData\LocalLow\DefaultCompany\PolyHaven
    private string PathS => Application.persistentDataPath + "/settings.json";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private string GetGameDataSelectedPath()
    {
        return Path.Combine(Application.persistentDataPath, $"{selectedVersion.nameFile}.json");
    }

    private void ResetSelectedVersionAndGameData()
    {
        selectedVersion = null;
        gameData = null;
    }

    public void LoadSettingsData()
    {
        if (File.Exists(PathS))
        {
            string json = File.ReadAllText(PathS);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            settingsData = new SettingsData();
        }
    }

    public void SaveSettingsData()
    {
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(PathS, json);
    }

    // GameData
    public void LoadGameData()
    {
        string PathG = GetGameDataSelectedPath();

        if (File.Exists(PathG))
        {
            string json = File.ReadAllText(PathG);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData(initDataSO);
        }
    }

    public void SaveGameData(GameData data)
    {
        string PathG = GetGameDataSelectedPath();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathG, json);

        ResetSelectedVersionAndGameData();
    }
}

[Serializable]
public class SettingsData
{
    public int lookSensitivity;
    public int fpsDropdownValue;
    public bool vSync;
    public SoundData soundData;

    public List<Versions> versions;

    public SettingsData()
    {
        lookSensitivity = 2;
        fpsDropdownValue = 3;
        vSync = true;
        soundData = new SoundData();

        versions = new List<Versions>();
    }
}

[Serializable]
public class Versions
{
    public string nameFile;
    public string nameVersion;
    public DateTime lastModified;

    public Versions(string fileName, string versionName)
    {
        nameFile = fileName;
        nameVersion = versionName;
        lastModified = DateTime.Now;
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