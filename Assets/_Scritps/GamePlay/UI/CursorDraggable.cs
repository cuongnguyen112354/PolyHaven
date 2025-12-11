using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CursorDraggable : MonoBehaviour
{ 
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    void Update()
    {
        if (IsPointerOverUIWithComponent<DraggableItem>())
            CursorManager.ChangeCursorIcon("none_drag");
        else
            CursorManager.DefaultCursorIcon();
    }

    bool IsPointerOverUIWithComponent<T>() where T : Component
    {
        PointerEventData pointerData = new(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<T>() != null)
            {
                return true;
            }
        }
        return false;
    }
}
