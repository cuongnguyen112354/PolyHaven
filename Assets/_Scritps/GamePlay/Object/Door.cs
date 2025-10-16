using NUnit.Framework;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent("PlayerController"))
        {
            doorAnimator.SetBool("IsOpen", !isOpen);
            isOpen = !isOpen;
        }
    }
}
