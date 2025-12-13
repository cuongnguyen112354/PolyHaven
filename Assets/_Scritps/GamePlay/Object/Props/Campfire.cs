using UnityEngine;

public class Campfire : MonoBehaviour
{
    private void OnDestroy()
    {
        if (PlayerHealth.Instance.isResting)
        {
            PlayerHealth.Instance.isResting = false;
        }            
    }

    private void OnTriggerStay(Collider other)
    {
        other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);

        if (playerHealth)
            playerHealth.isResting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);

        if (playerHealth)
            playerHealth.isResting = false;
    }
}
