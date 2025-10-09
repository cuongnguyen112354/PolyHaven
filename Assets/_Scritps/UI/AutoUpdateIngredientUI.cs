using UnityEngine;

public class AutoUpdateIngredientUI : MonoBehaviour
{
    void OnEnable()
    {
        if (CraftingManager.Instance != null)
            CraftingManager.Instance.UpdateRecipeUI();
    }
}
