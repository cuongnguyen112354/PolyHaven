using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Image iconItem;

    [SerializeField] private TMP_Text quanlity;
    [SerializeField] private Color enoughColor;
    [SerializeField] private Color noneEnoughColor;

    // private Ingredient ingredient = null;

    public bool Init(Ingredient ingredient)
    {
        // this.ingredient = ingredient;

        iconItem.sprite = ingredient.item.icon;

        (bool result, int missingQuantity) = InventoryManager.Instance.HasItem(ingredient);
        if (result)
        {
            canvasGroup.alpha = 1;
            quanlity.color = enoughColor;
            quanlity.text = ingredient.quantity.ToString();

            return true;
        }
        else
        {
            canvasGroup.alpha = .6f;
            quanlity.color = noneEnoughColor;
            quanlity.text = $"{missingQuantity}/{ingredient.quantity}";

            return false;
        }
    }

    // private void OnEnable()
    // {
    //     iconItem.sprite = ingredient.item.icon;

    //     (bool result, int missingQuantity) = InventoryManager.Instance.HasItem(ingredient);
    //     if (result)
    //     {
    //         canvasGroup.alpha = 1;
    //         quanlity.color = enoughColor;
    //         quanlity.text = ingredient.quantity.ToString();
    //     }
    //     else
    //     {
    //         canvasGroup.alpha = .6f;
    //         quanlity.color = noneEnoughColor;
    //         quanlity.text = $"{missingQuantity}/{ingredient.quantity}";
    //     }
    // }
}
