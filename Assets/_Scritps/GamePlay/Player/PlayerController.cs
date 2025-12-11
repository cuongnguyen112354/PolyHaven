using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Transform handPosition;

    [HideInInspector] public bool onDragging = false;

    [SerializeField] private HotBar hotBar;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.UI.Slot1.performed += ctx => hotBar.SelectingSlot(0);
        inputActions.UI.Slot2.performed += ctx => hotBar.SelectingSlot(1);
        inputActions.UI.Slot3.performed += ctx => hotBar.SelectingSlot(2);
        inputActions.UI.Slot4.performed += ctx => hotBar.SelectingSlot(3);
        inputActions.UI.Slot5.performed += ctx => hotBar.SelectingSlot(4);
        inputActions.UI.Slot6.performed += ctx => hotBar.SelectingSlot(5);
    }

    void OnDisable()
    {
        inputActions.UI.Slot1.performed += ctx => hotBar.SelectingSlot(0);
        inputActions.UI.Slot2.performed += ctx => hotBar.SelectingSlot(1);
        inputActions.UI.Slot3.performed += ctx => hotBar.SelectingSlot(2);
        inputActions.UI.Slot4.performed += ctx => hotBar.SelectingSlot(3);
        inputActions.UI.Slot5.performed += ctx => hotBar.SelectingSlot(4);
        inputActions.UI.Slot6.performed += ctx => hotBar.SelectingSlot(5);

        inputActions.Disable();
    }

    void Update()
    {
        if (GameManager.Instance.CompareGameState("Playing"))
        {
            SelectingSlot();
        }

        if (onDragging)
        {
            CursorManager.ChangeCursorIcon("drag", true);
            return;
        }
    }

    private void SelectingSlot()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y < 0)
                hotBar.ChangeOneSlot(true);
            else
                hotBar.ChangeOneSlot(false);
        }
    }
}
