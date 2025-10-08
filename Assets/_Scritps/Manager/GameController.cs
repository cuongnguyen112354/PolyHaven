using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] TMP_Text actualyFPSText;

    private int frameCount = 0;
    private float elapsedTime = 0f;
    private readonly float refreshTime = .5f; // Cập nhật mỗi 1 giây

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
        CursorManager.Init();
        DataPersistence.Instance.LoadGameData();

        GameManager.Instance.StartGame();
    }

    void Update()
    {
        if (GameManager.Instance.CompareGameState("Loading")) return;

        if (!GameManager.Instance.CompareGameState("Pause"))
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                GameManager.Instance.ActiveUI("Inventory");

            if (Input.GetKeyDown(KeyCode.Q))
                GameManager.Instance.ActiveUI("Selection");
        }

        if (GameManager.Instance.CompareGameState("Playing"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.SetGameState(GameManager.GameState.Paused);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ResumeGame();
        }

        frameCount++;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= refreshTime)
        {
            int fps = Mathf.RoundToInt(frameCount / elapsedTime);
            actualyFPSText.text = "FPS: " + fps;

            // Reset đếm
            frameCount = 0;
            elapsedTime = 0f;
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }

    public void ReloadGame()
    {
        GameManager.Instance.Reload();
        Time.timeScale = 1f; // Resume the game
    }

    public void ExitGame()
    {
        DataPersistence.Instance.SaveGameData();
        MyLibrary.ControlUtils.QuitGame();
    }
}
