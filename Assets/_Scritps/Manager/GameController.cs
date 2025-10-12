using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] TMP_Text actualyFPSText;

    private int frameCount = 0;
    private float elapsedTime = 0f;
    private readonly float refreshTime = .5f; // Cập nhật mỗi 1 giây

    private InputSystem_Actions inputActions;

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

        inputActions = new InputSystem_Actions();
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

    void Start()
    {
        CursorManager.Init();
        DataPersistence.Instance.LoadGameData();

        GameManager.Instance.StartGame();
    }

    void Update()
    {
        if (GameManager.Instance.CompareGameState("Loading")) return;

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

    private void OnInventory()
    {
        if (GameManager.Instance.CompareGameState("Loading")) return;

        if (GameManager.Instance.CompareGameState("Playing"))
            GameManager.Instance.ActiveUI("Inventory");
        else if (GameManager.Instance.CompareGameState("UI"))
            ResumeGame();
    }

    private void OnCrafting()
    {
        if (GameManager.Instance.CompareGameState("Loading")) return;

        if (GameManager.Instance.CompareGameState("Playing"))
            GameManager.Instance.ActiveUI("Crafting");
        else if (GameManager.Instance.CompareGameState("UI"))
            ResumeGame();
    }

    private void OnPause()
    {
        if (GameManager.Instance.CompareGameState("Loading")) return;

        if (GameManager.Instance.CompareGameState("Playing"))
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);
        else
            ResumeGame();
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
