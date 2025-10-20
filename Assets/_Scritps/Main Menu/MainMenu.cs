using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] List<VersionUI> versionUIs;
    [SerializeField] private Button createVersionBtn;

    [SerializeField] private Button enterGameBtn;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (!DataPersistence.Instance.isLoaded)
            DataPersistence.Instance.LoadSettingsData();

        SetVersionUIs();
    }

    private void SetVersionUIs()
    {
        List<Versions> versions = DataPersistence.Instance.GetSettingsData().versions;

        if (versionUIs.Count > versions.Count)
            createVersionBtn.gameObject.SetActive(true);

        for (int i = 0; i < versions.Count; i++)
            versionUIs[i].Init(versions[i]);
    }
    
    public void InteractiveEnterBtn()
    {
        enterGameBtn.interactable = true;
    }

    public void EnterGame()
    {

        GameController.Instance.GamePlayScene();
    }

    public void ExitGame()
    {
        GameController.Instance.ExitGame();
    }
}
