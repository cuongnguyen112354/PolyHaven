using UnityEngine;

public abstract class AbTool : MonoBehaviour, IEquipment
{
    [SerializeField] private Animator animator;

    protected ToolSO toolSO;

    private bool isUsing, usingBtnDown = false;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void Update()
    {
        Use();
    }

    void OnEnable()
    {
        isUsing = false;

        inputActions.Enable();

        inputActions.Equipment.Use.started += _ => { usingBtnDown = true; };
        inputActions.Equipment.Use.canceled += _ => { usingBtnDown = false; };
    }

    void OnDisable()
    {
        inputActions.Equipment.Use.started -= _ => { usingBtnDown = true; };
        inputActions.Equipment.Use.canceled -= _ => { usingBtnDown = false; };

        inputActions.Disable();
    }

    private void Use()
    {
        if (GameManager.Instance.CompareGameState("Playing"))
        {
            if (usingBtnDown && !isUsing)
            {
                isUsing = true;

                animator.SetTrigger("use");
            }
        }
    }

    private void DoneUsingEvent()
    {
        isUsing = false;
    }

    public void Init(ToolSO toolSO)
    {
        this.toolSO = toolSO;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    public abstract void HitEvent();
}
