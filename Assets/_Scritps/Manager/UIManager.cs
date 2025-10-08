using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputSystem_Actions))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private UIAction[] uiActions;

    private Dictionary<string, GameObject> uiActionDict = new();
    private GameObject currentPanel;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        foreach (UIAction action in uiActions)
            uiActionDict.TryAdd(action.panelName, action.panel);

        inputActions.UI.Inventory.performed += _ => OpenInventory();
    }

    private void OpenInventory()
    {
        if (uiActionDict.TryGetValue("Inventory", out GameObject panel))
        {
            if (currentPanel == null)
            {
                currentPanel = panel;
                currentPanel.SetActive(true);
            }
            else
            {
                currentPanel.SetActive(false);

                currentPanel = panel;
                currentPanel.SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class UIAction
{
    public GameObject panel;
    public string panelName;
}