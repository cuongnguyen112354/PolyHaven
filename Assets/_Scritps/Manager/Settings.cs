using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    [SerializeField] Slider lookSensitivitySlider;
    [SerializeField] TMP_Text lookSensitivityValue;

    [SerializeField] TMP_Dropdown fpsDropdown;
    [SerializeField] Toggle qualityToggle;

    public float LookSensitivity
    {
        get { return lookSensitivitySlider.value; }
        private set { }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(SettingsData settingsData)
    {
        lookSensitivitySlider.onValueChanged.AddListener(value => { lookSensitivityValue.text = value.ToString(); });
        fpsDropdown.onValueChanged.AddListener(_ =>
        {
            Application.targetFrameRate = int.Parse(fpsDropdown.options[fpsDropdown.value].text);
        });
        qualityToggle.onValueChanged.AddListener(isOn => { QualitySettings.vSyncCount = isOn ? 1 : 0; });

        lookSensitivitySlider.value = settingsData.lookSensitivity;
        fpsDropdown.value = settingsData.fpsDropdownValue;
        qualityToggle.isOn = settingsData.vSync;

        AudioManager.Instance.Init(settingsData.soundData);
    }

    public SettingsData GetSettingsData()
    {
        return new SettingsData
        {
            lookSensitivity = (int)lookSensitivitySlider.value,
            fpsDropdownValue = fpsDropdown.value,
            vSync = qualityToggle.isOn,
            soundData = AudioManager.Instance.GetSoundData()
        };
    }
}

[System.Serializable]
public class SettingsData
{
    public int lookSensitivity;
    public int fpsDropdownValue;
    public bool vSync;
    public SoundData soundData;

    public SettingsData()
    {
        lookSensitivity = 2;
        fpsDropdownValue = 3;
        vSync = true;
        soundData = new SoundData();
    }
}