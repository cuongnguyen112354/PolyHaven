using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private SettingsData settingsData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CursorManager.Init();
        // DataPersistence.Instance.LoadGameData();
    }

    public SettingsData GetSettingsData()
    {
        return settingsData;
    }

    public void SetSettingsData(SettingsData data)
    {
        settingsData = data;
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void GamePlayScene()
    {
        SceneManager.LoadScene("PolyHaven");
    }

    public void ExitGame()
    {
        // DataPersistence.Instance.SaveGameData();
        DataPersistence.Instance.SaveSettingsData();
        MyLibrary.ControlUtils.QuitGame();
    }
}
