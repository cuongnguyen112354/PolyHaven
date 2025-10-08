using UnityEngine;

public class ConstructionMaterials : MonoBehaviour
{
    [SerializeField] private Ingredient[] ingredients;

    void OnEnable()
    {
        ingredients = Resources.Load<BlueprintSO>($"_Items/{gameObject.name}").ingredients;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (Ingredient ingredient in ingredients)
            {
                print("Ingredient: " + ingredient.item.itemName + ", Quantity: " + ingredient.quantity);
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            ingredients[0].quantity += 1; // Example of modifying an ingredient
            print("Updated Ingredient: " + ingredients[0].item.itemName + ", New Quantity: " + ingredients[0].quantity);
        }
    }

    public void AddIngredient(Ingredient ingredient)
    {
        // Logic to add an ingredient to the construction materials
    }
}
