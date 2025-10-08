using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeSelector : MonoBehaviour
{
    [SerializeField] private Image iconItemResult;

    private CraftingRecipe craftingRecipe;

    public void Init(CraftingRecipe recipe)
    {
        craftingRecipe = recipe;

        iconItemResult.sprite = craftingRecipe.result.icon;
    }

    public void SelectRecipe()
    {
        CraftingManager.Instance.SetSelectingRecipe(craftingRecipe);
    }
}
