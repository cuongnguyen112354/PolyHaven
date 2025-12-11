using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;

    private Transform rootTransform;

    private Image image;

    public void Init(Sprite sprite)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        image.sprite = sprite;

        rootTransform = RootTransform.Instance.rootTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(rootTransform);
        transform.SetAsLastSibling();
        parentAfterDrag.GetChild(0).gameObject.SetActive(false);
        image.raycastTarget = false;

        AudioManager.Instance.PlayAudioClip("put_up");
        PlayerController.Instance.onDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentAfterDrag.GetChild(0).gameObject.SetActive(true);
        transform.SetParent(parentAfterDrag);
        transform.SetAsFirstSibling();
        image.raycastTarget = true;

        AudioManager.Instance.PlayAudioClip("put_down");
        PlayerController.Instance.onDragging = false;
    }
}
