using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

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
        DataPersistence.Instance.SaveSettingsData();
        MyLibrary.ControlUtils.QuitGame();
    }
}
