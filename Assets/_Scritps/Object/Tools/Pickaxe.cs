using UnityEngine;

public class Pickaxe : AbTool
{
    public override void HitEvent()
    {
        if (InteractObject.focusingObject &&
            InteractObject.focusingObject.TryGetComponent<MineableObject>(out var mineableObject))
        {
            mineableObject.Affected(toolSO.damage);
        }
    }
}
