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

    void LoadingState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void PlayingState()
    {
        // darkBg.SetActive(false);
        // pauseMenu.SetActive(false);
        // inventoryPanel.SetActive(false);
        // selectionPanel.SetActive(false);

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
        fadeAnimator.SetFloat("speed", 1f);
        fadeAnimator.Play("FadeIn", 0, 0f);

        yield return new WaitForSeconds(1);
        SetGameState(GameState.Playing);
    }

    IEnumerator FadeOut(Func<IEnumerator> callback)
    {
        fadeAnimator.Play("FadeIn", 0, 1f);
        fadeAnimator.SetFloat("speed", -1f);

        yield return new WaitForSeconds(1);
        StartCoroutine(callback());
    }

    public void ActiveUI(string panelName)
    {
        if (panelName == "Inventory")
            inventoryPanel.SetActive(true);
        else if (panelName == "Selection")
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

    public void Reload()
    {
        SetGameState(GameState.Loading);

        StartCoroutine(FadeOut(() => { return FadeIn(); }));
    }
}   