using UnityEngine;
using System.IO;

public class LoadAndLoadSettings
{
    public static LoadAndLoadSettings Instance;
    // C:\Users\Chi Cuong\AppData\LocalLow\DefaultCompany\PolyHaven\settings.json
    private static string Path => Application.persistentDataPath + "/settings.json";

    public static void SaveSettingsData(SettingsData settingsData)
    {
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(Path, json);
    }

    public static SettingsData LoadSettingsData()
    {
        if (File.Exists(Path))
        {
            string json = File.ReadAllText(Path);
            SettingsData settingsData = JsonUtility.FromJson<SettingsData>(json);

            return settingsData;
        }
        else
        {
            return new SettingsData();
        }
    }
}