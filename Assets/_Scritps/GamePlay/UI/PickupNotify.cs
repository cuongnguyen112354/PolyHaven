using TMPro;
using UnityEngine;

public class PickupNotify : MonoBehaviour
{
    [SerializeField] private TMP_Text pickupText;
    [SerializeField] private Animator pickupAnim;

    private void DoneFloatingEvent()
    {
        gameObject.SetActive(false);
    }

    public void ShowPickupNotification(int quantity, string itemName)
    {
        gameObject.SetActive(true);

        pickupText.text = $"+{quantity} {itemName}";
        pickupAnim.Play("Floating", 0, 0f);

    }
}
