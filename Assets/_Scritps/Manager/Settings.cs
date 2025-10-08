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

    void Start()
    {
        lookSensitivitySlider.onValueChanged.AddListener(value => { lookSensitivityValue.text = value.ToString(); });
        lookSensitivitySlider.value = 2;

        fpsDropdown.onValueChanged.AddListener(_ =>
        {
            Application.targetFrameRate = int.Parse(fpsDropdown.options[fpsDropdown.value].text);
        });
        fpsDropdown.value = 3;

        QualitySettings.vSyncCount = 1;
        qualityToggle.onValueChanged.AddListener(isOn => { QualitySettings.vSyncCount = isOn ? 1 : 0; });
    }    
}
