using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [Header("----------CraftingRecipes SO----------")]
    [SerializeField] private CraftingRecipesSO craftingRecipes;

    [SerializeField] private GameObject craftRecipePrefab;

    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Image iconItemResult;
    [SerializeField] private GameObject[] ingredientGOs;

    [Header("----------Tabs Menu----------")]
    [SerializeField] private Transform toolTab;
    [SerializeField] private Transform propTab;
    [SerializeField] private Transform houseTab;

    [SerializeField] private Button craftButton;

    private CraftingRecipe recipeSelecting;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (CraftingRecipe craftingRecipe in craftingRecipes.toolRecipes)
        {
            GameObject craftRecipe = Instantiate(craftRecipePrefab);
            craftRecipe.transform.SetParent(toolTab);
            craftRecipe.transform.localScale = Vector3.one;

            // craftRecipe.GetComponent<Button>().interactable = craftingRecipe.isOwned;
            craftRecipe.GetComponent<CraftRecipeSelector>().Init(craftingRecipe);
        }

        foreach (CraftingRecipe craftingRecipe in craftingRecipes.propRecipes)
        {
            GameObject craftRecipe = Instantiate(craftRecipePrefab);
            craftRecipe.transform.SetParent(propTab);
            craftRecipe.transform.localScale = Vector3.one;

            // craftRecipe.GetComponent<Button>().interactable = craftingRecipe.isOwned;
            craftRecipe.GetComponent<CraftRecipeSelector>().Init(craftingRecipe);
        }

        foreach (CraftingRecipe craftingRecipe in craftingRecipes.houseRecipes)
        {
            GameObject craftRecipe = Instantiate(craftRecipePrefab);
            craftRecipe.transform.SetParent(houseTab);
            craftRecipe.transform.localScale = Vector3.one;

            // craftRecipe.GetComponent<Button>().interactable = craftingRecipe.isOwned;
            craftRecipe.GetComponent<CraftRecipeSelector>().Init(craftingRecipe);
        }
    }

    private void UpdateSelectingRecipeUI(CraftingRecipe recipe)
    {
        iconItemResult.sprite = recipe.result.icon;

        int index = 0;
        bool craftBtnStatus = true;
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (!ingredientGOs[index].GetComponent<IngredientUI>().Init(ingredient))
                craftBtnStatus = false;
            ingredientGOs[index++].SetActive(true);
        }

        craftButton.interactable = craftBtnStatus;

        for (int i = index; i < 3; i++)
        {
            ingredientGOs[i].SetActive(false);
        }
    }

    public void UpdateRecipeUI()
    {
        if (recipeSelecting == null) return;

        int index = 0;
        bool craftBtnStatus = true;
        foreach (Ingredient ingredient in recipeSelecting.ingredients)
        {
            if (!ingredientGOs[index].GetComponent<IngredientUI>().Init(ingredient))
                craftBtnStatus = false;
            ingredientGOs[index++].SetActive(true);
        }

        craftButton.interactable = craftBtnStatus;
    }

    public void ResetUI()
    {
        iconItemResult.sprite = defaultIcon;

        foreach (GameObject ingredient in ingredientGOs)
            ingredient.SetActive(false);

        craftButton.interactable = false;
    }

    public void SetSelectingRecipe(CraftingRecipe recipe)
    {
        recipeSelecting = recipe;

        UpdateSelectingRecipeUI(recipe);
    }

    public void CraftItemResult()
    {
        if (InventoryManager.Instance.IsAddItem(recipeSelecting.result))
        {
            InventoryManager.Instance.RemoveItems(recipeSelecting.ingredients);
            ResetUI();
        }
    }
}
