using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputSystem_Actions))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject chestPanel;
    [SerializeField] private GameObject craftingPanel;

    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private TMP_Text confirmContent;
    [SerializeField] private Button confirmBtn;
    private Func<UniTaskVoid> onConfirm;

    [SerializeField] private GameObject darkBg;
    [SerializeField] private Animator fadeAnimator;

    [SerializeField] private ReceivedNotify receivedNotify;
    
    private WaitForSeconds _waitForSeconds1 = new (1);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    IEnumerator FadeIn()
    {
        // fadeAnimator.SetFloat("speed", 1f);
        fadeAnimator.Play("FadeIn", 0, 0f);

        yield return _waitForSeconds1;
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
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

    private void OnConfirm()
    {
        onConfirm?.Invoke().Forget();
        onConfirm = null;
    }

    public void OnFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void OnFadeOutCoroutine()
    {
        StartCoroutine(FadeOut(() => { return FadeIn(); }));
    }

    public void OnFadeOutFunction()
    {
        StartCoroutine(FadeOut(() => GameManager.Instance.ExitScene()));
    }

    public void OnStart()
    {
        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false);

        if (inventoryPanel.activeSelf)
            inventoryPanel.SetActive(false);

        if (chestPanel.activeSelf)
            chestPanel.SetActive(false);

        if (craftingPanel.activeSelf)
            craftingPanel.SetActive(false);

        if (confirmPopup.activeSelf)
            confirmPopup.SetActive(false);

        if (!fadeAnimator.gameObject.activeSelf)
            fadeAnimator.gameObject.SetActive(true);        

        GameManager.Instance.SetGameState(GameManager.GameState.Loading);
        StartCoroutine(FadeIn());
    }

    public void OnPlaying()
    {
        if (chestPanel.activeSelf)
        {
            Storage chest = StorageCodeMap.GetComponentByCode(ChestManager.Instance.currentChestCode);
            if (chest is Chest chestObj)
                chestObj.Affected();
            chestPanel.SetActive(false);
        }

        darkBg.SetActive(false);
        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void OnPause()
    {
        pauseMenu.SetActive(true);
    }

    public void ActiveUI(string panelName)
    {
        if (panelName == "Inventory")
            inventoryPanel.SetActive(true);
        else if (panelName == "Chest")
        {
            chestPanel.SetActive(true);
            inventoryPanel.SetActive(true);
        }
        else if (panelName == "Crafting")
            craftingPanel.SetActive(true);

        darkBg.SetActive(true);
    }

    public void ShowPickupNotify(int quantity, string itemName)
    {
        receivedNotify.ShowRecievedNotification(quantity, itemName);
    }

    public void ShowConfirmPopup(string content, Func<UniTaskVoid> action)
    {
        confirmContent.text = content;
        onConfirm = action;

        confirmPopup.SetActive(true);
        GameManager.Instance.SetGameState(GameManager.GameState.UI);
    }
}