using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Transform handPosition;

    private InputSystem_Actions inputActions;
    private InventoryManager inventoryManager;

    private const int offset = 20;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.UI.Slot1.performed += ctx => inventoryManager.SelectingSlot(1 + offset);
        inputActions.UI.Slot2.performed += ctx => inventoryManager.SelectingSlot(2 + offset);
        inputActions.UI.Slot3.performed += ctx => inventoryManager.SelectingSlot(3 + offset);
        inputActions.UI.Slot4.performed += ctx => inventoryManager.SelectingSlot(4 + offset);
        inputActions.UI.Slot5.performed += ctx => inventoryManager.SelectingSlot(5 + offset);
        inputActions.UI.Slot6.performed += ctx => inventoryManager.SelectingSlot(6 + offset);
    }

    void OnDisable()
    {
        inputActions.UI.Slot1.performed -= ctx => inventoryManager.SelectingSlot(1 + offset);
        inputActions.UI.Slot2.performed -= ctx => inventoryManager.SelectingSlot(2 + offset);
        inputActions.UI.Slot3.performed -= ctx => inventoryManager.SelectingSlot(3 + offset);
        inputActions.UI.Slot4.performed -= ctx => inventoryManager.SelectingSlot(4 + offset);
        inputActions.UI.Slot5.performed -= ctx => inventoryManager.SelectingSlot(5 + offset);
        inputActions.UI.Slot6.performed -= ctx => inventoryManager.SelectingSlot(6 + offset);

        inputActions.Disable();
    }

    void Update()
    {
        if (GameManager.Instance.CompareGameState("Playing"))
        {
            SelectingSlot();
        }
    }

    private void SelectingSlot()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y < 0)
                InventoryManager.Instance.ChangeOneSlot(true);
            else
                InventoryManager.Instance.ChangeOneSlot(false);
        }
    }
}
