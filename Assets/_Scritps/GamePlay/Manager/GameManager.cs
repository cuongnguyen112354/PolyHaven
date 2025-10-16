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
    GameState CurrentGameState;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] GameObject darkBg;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject selectionPanel;

    [SerializeField] Animator fadeAnimator;

    [SerializeField] TMP_Text actualyFPSText;

    private int frameCount = 0;
    private float elapsedTime = 0f;
    private readonly float refreshTime = .5f; // Cập nhật mỗi 1 giây

    private WaitForSeconds _waitForSeconds1 = new (1);

    private InputSystem_Actions inputActions;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        DataPersistence.Instance.LoadGameData();

        StartGame();
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
    }

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

    void LoadingState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void PlayingState()
    {
        darkBg.SetActive(false);
        inventoryPanel.SetActive(false);
        selectionPanel.SetActive(false);
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
    }

    void UIState()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void PausedState()
    {
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }

    IEnumerator FadeIn()
    {
        // fadeAnimator.SetFloat("speed", 1f);
        fadeAnimator.Play("FadeIn", 0, 0f);

        yield return _waitForSeconds1;

        SetGameState(GameState.Playing);
    }

    IEnumerator FadeOut(Func<IEnumerator> callback = null)
    {
        fadeAnimator.Play("FadeOut", 0, 0f);
        // fadeAnimator.SetFloat("speed", -1f);

        yield return _waitForSeconds1;
        if (callback != null)
            StartCoroutine(callback());
    }

    IEnumerator FadeOut(Action callback = null)
    {
        fadeAnimator.Play("FadeOut", 0, 0f);
        // fadeAnimator.SetFloat("speed", -1f);

        yield return _waitForSeconds1;
        callback?.Invoke();
    }

    private void ExitScene()
    {
        Cursor.lockState = CursorLockMode.None;
        GameController.Instance.MainMenuScene();
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
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

    public void ActiveUI(string panelName)
    {
        if (panelName == "Inventory")
            inventoryPanel.SetActive(true);
        else if (panelName == "Crafting")
            selectionPanel.SetActive(true);

        darkBg.SetActive(true);

        SetGameState(GameState.UI);
    }

    public void StartGame()
    {
        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false);

        if (inventoryPanel.activeSelf)
            inventoryPanel.SetActive(false);

        if (selectionPanel.activeSelf)
            selectionPanel.SetActive(false);

        if (!fadeAnimator.gameObject.activeSelf)
            fadeAnimator.gameObject.SetActive(true);

        SetGameState(GameState.Loading);
        StartCoroutine(FadeIn());
    }

    public bool CompareGameState(string state)
    {
        return CurrentGameState.ToString() == state;
    }

    public void ReloadGamePaly()
    {
        SetGameState(GameState.Loading);
        StartCoroutine(FadeOut(() => { return FadeIn(); }));
        
        Time.timeScale = 1f; // Resume the game
    }

    public void ExitGamePlay()
    {
        SetGameState(GameState.Loading);

        Time.timeScale = 1f;
        DataPersistence.Instance.SaveGameData();
        GameController.Instance.SetSettingsData(Settings.Instance.GetSettingsData());
        
        StartCoroutine(FadeOut(() => ExitScene()));
    }
}   