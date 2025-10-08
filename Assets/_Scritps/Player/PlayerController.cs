using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Transform handPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        else
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    InventoryManager.Instance.SelectingSlot(i);
            }
    }
}
