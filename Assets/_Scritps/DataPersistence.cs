using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class DataPersistence : MonoBehaviour
{
    public static DataPersistence Instance;
    public bool isLoaded = false;

    [SerializeField] private InitDataSO initDataSO;

    private SettingsData settingsData;
    private GameData gameData;

    private Versions selectedVersion;

    // C:\Users\Chi Cuong\AppData\LocalLow\DefaultCompany\PolyHaven
    private string PathS => Application.persistentDataPath + "/settings.json";

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        isLoaded = false;
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

    public SettingsData GetSettingsData()
    {
        return settingsData;
    }

    public void SetSettingsData(SettingsData data)
    {
        data.versions = settingsData.versions;
        settingsData = data;
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public void SetGameData(GameData data)
    {
        gameData = data;
    }

    public Versions GetVersionsSelected()
    {
        return selectedVersion;
    }

    public void SetVersionsSelected(Versions version)
    {
        selectedVersion = version;
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

        isLoaded = true;
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
    public List<EnvironmentObject> environmentData;
    public List<ConstructionObject> constructionData;

    public GameData(
        PlayerData playerData,
        List<SlotData> inventoryData,
        List<EnvironmentObject> environmentData,
        List<ConstructionObject> constructionData
    )
    {
        this.playerData = playerData;
        this.inventoryData = inventoryData;
        this.environmentData = environmentData;
        this.constructionData = constructionData;
    }

    public GameData(InitDataSO initDataSO)
    {
        playerData = initDataSO.playerData;
        inventoryData = initDataSO.inventoryData;
        environmentData = initDataSO.environmentData;
        constructionData = initDataSO.constructionData;
    }
}