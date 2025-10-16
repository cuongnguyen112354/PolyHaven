using UnityEngine;

public class Campfire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
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
