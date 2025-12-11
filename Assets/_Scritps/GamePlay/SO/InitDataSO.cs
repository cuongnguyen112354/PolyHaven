using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InitDataSO", menuName = "ScriptableObjects/InitDataSO")]
public class InitDataSO : ScriptableObject
{
    // public SettingsData settingsData;
    public PlayerData playerData;
    public List<SlotData> hotBarData;
    public List<SlotData> inventoryData;
    public List<ChestData> chestsData;
    public List<EnvironmentObject> environmentData;
    public List<ConstructionObject> constructionData;
}