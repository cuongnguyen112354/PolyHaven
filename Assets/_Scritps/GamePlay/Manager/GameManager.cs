using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public  TMP_Text interactionInfo_Text;

    public enum GameState
    {
        Loading,
        Playing,
        UI,
        Paused
    }
    [SerializeField] private GameState CurrentGameState;

    [SerializeField] TMP_Text actualyFPSText;

    private int frameCount = 0;
    private float elapsedTime = 0f;
    private readonly float refreshTime = .5f; // Cập nhật mỗi 1 giây

    private InputSystem_Actions inputActions;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        DataManager.Instance.InitData();
        UIManager.Instance.OnStart();
    }

    void Update()
    {
        if (CompareGameState("Loading")) return;

        frameCount++;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= refreshTime)
        {
            int fps = Mathf.RoundToInt(frameCount / elapsedTime);
            actualyFPSText.text = $"{fps}";

            // Reset đếm
            frameCount = 0;
            elapsedTime = 0f;
        }
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.UI.Inventory.performed += ctx => OnInventory();
        inputActions.UI.Crafting.performed += ctx => OnCrafting();
        inputActions.UI.Escape.performed += ctx => OnPause();
    }

    void OnDisable()
    {
        inputActions.UI.Inventory.performed -= ctx => OnInventory();
        inputActions.UI.Crafting.performed -= ctx => OnCrafting();
        inputActions.UI.Escape.performed -= ctx => OnPause();

        inputActions.Disable();

        StorageCodeMap.codeMap.Clear();
    }

    // Những hàm kích hoạt cho InputSystem
    private void OnInventory()
    {
        if (CompareGameState("Loading")) return;

        if (CompareGameState("Playing"))
            ActiveUI("Inventory");
        else if (CompareGameState("UI"))
            ResumeGame();
    }

    private void OnCrafting()
    {
        if (CompareGameState("Loading")) return;

        if (CompareGameState("Playing"))
            ActiveUI("Crafting");
        else if (CompareGameState("UI"))
            ResumeGame();
    }

    private void OnPause()
    {
        if (CompareGameState("Loading")) return;

        if (CompareGameState("Playing"))
            SetGameState(GameState.Paused);
        else
            ResumeGame();
    }

    // Những cái State
    void LoadingState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void PlayingState()
    {
        UIManager.Instance.OnPlaying();

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    void UIState()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void PausedState()
    {
        UIManager.Instance.OnPause();

        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }

    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.Loading:
                LoadingState();
                break;
            case GameState.Playing:
                PlayingState();
                break;
            case GameState.UI:
                UIState();
                break;
            case GameState.Paused:
                PausedState();
                break;
        }
    }

    public bool CompareGameState(string state)
    {
        return CurrentGameState.ToString() == state;
    }

    public void ActiveUI(string panelName)
    {
        UIManager.Instance.ActiveUI(panelName);

        SetGameState(GameState.UI);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }

    public void ReloadGamePaly()
    {
        SetGameState(GameState.Loading);
        UIManager.Instance.OnFadeOutCoroutine();
        
        Time.timeScale = 1f; // Resume the game
    }

    public void ExitGamePlay()
    {
        SetGameState(GameState.Loading);
        Time.timeScale = 1f;

        GameData gameData = DataManager.Instance.GetGameData();
        SettingsData settingsData = Settings.Instance.GetSettingsData();

        DataPersistence.Instance.SaveGameData(gameData);
        DataPersistence.Instance.SetSettingsData(settingsData);
        
        UIManager.Instance.OnFadeOutFunction();
    }

    public void ExitScene()
    {
        Cursor.lockState = CursorLockMode.None;
        GameController.Instance.MainMenuScene();
    }
}   