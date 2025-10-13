using UnityEngine;

public class Axe : AbTool
{
    public override void HitEvent()
    {
        if (InteractObject.focusingObject &&
            InteractObject.focusingObject.TryGetComponent<ChoppableObject>(out var choppableObject))
        {
            choppableObject.Affected(toolSO.damage);
        }
    }
}
