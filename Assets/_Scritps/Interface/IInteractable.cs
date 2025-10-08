using UnityEngine;

public interface IInteractable
{
    (Sprite, string) HowInteract();
    string GetItemName();
    void Interact();
}
