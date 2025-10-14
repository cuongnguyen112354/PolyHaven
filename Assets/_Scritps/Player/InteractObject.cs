using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractObject : MonoBehaviour
{
    [HideInInspector] public static GameObject focusingObject;

    [SerializeField] private float maxInteractionDistance = 1.75f;

    [Header("Icons")]
    [SerializeField] private Image targetIcon;
    [SerializeField] private Sprite defalutTargetIcon;

    private int layerMask;
    private TMP_Text interactionInfo_Text;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Pickup.performed += _ => Pickup();
    }

    void OnDisable()
    {
        inputActions.Player.Pickup.performed -= _ => Pickup();

        inputActions.Disable();
    }

    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (!GameManager.Instance.CompareGameState("Playing") ||
            !Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxInteractionDistance, layerMask, QueryTriggerInteraction.Ignore) ||
            hit.collider.gameObject.GetComponent<IInteractable>() == null)
        {
            focusingObject = null;
            
            interactionInfo_Text.gameObject.SetActive(false);

            targetIcon.sprite = defalutTargetIcon;
            targetIcon.SetNativeSize();

            return;
        }

        focusingObject = hit.collider.gameObject;

        (targetIcon.sprite, interactionInfo_Text.text) = focusingObject.GetComponent<IInteractable>().HowInteract();
        targetIcon.SetNativeSize();
        interactionInfo_Text.gameObject.SetActive(true);
    }

    private void Pickup()
    {
        if (focusingObject == null || !GameManager.Instance.CompareGameState("Playing")) return;

        if (focusingObject.TryGetComponent<PickableObject>(out var item))
            item.Affected(0);
        else if (focusingObject.TryGetComponent<RetrievableObject>(out var prop))
            prop.Affected(0);
    }

    public void SetInteractionText(TMP_Text _TMP_Text)
    {
        interactionInfo_Text = _TMP_Text;
    }
}