using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISubInteractable
{
    void HowInteract();
    UniTask Destroy();
}
