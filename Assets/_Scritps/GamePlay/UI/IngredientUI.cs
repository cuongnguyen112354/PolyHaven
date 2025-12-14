using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] private Image iconItem;
    [SerializeField] private TMP_Text quanlity;

    [SerializeField] private Color enoughColor;
    [SerializeField] private Color noneEnoughColor;

    public bool Init(Ingredient ingredient)
    {
        iconItem.sprite = ingredient.item.icon;

        int qtyInInven = Inventory.Instance.GetQuantityItem(ingredient.item.itemName);
        int qtyInHotBar = HotBar.Instance.GetQuantityItem(ingredient.item.itemName);

        if (qtyInInven + qtyInHotBar >= ingredient.quantity)
            return UpdateIngredientUI(ingredient, true, qtyInInven + qtyInHotBar);
        else
            return UpdateIngredientUI(ingredient, false, qtyInInven + qtyInHotBar);
    }

    public bool UpdateIngredientUI(Ingredient ingredient, bool result, int missingQuantity)
    {
        if (result)
            quanlity.color = enoughColor;
        else
            quanlity.color = noneEnoughColor;
        
        quanlity.text = $"{missingQuantity}/{ingredient.quantity}";
        return result;
    }
}
