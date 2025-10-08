using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDragEnd : MonoBehaviour, IEndDragHandler
{
    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.Instance.PlayAudioClip("pick_up");
    }
}