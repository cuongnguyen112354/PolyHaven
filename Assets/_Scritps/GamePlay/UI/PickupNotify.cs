using TMPro;
using UnityEngine;

public class ReceivedNotify : MonoBehaviour
{
    [SerializeField] private TMP_Text receivedText;
    [SerializeField] private Animator receivedAnim;

    private void DoneFloatingEvent()
    {
        gameObject.SetActive(false);
    }

    public void ShowRecievedNotification(int quantity, string itemName)
    {
        gameObject.SetActive(true);

        receivedText.text = $"+{quantity} {itemName}";
        receivedAnim.Play("Floating", 0, 0f);
    }
}
