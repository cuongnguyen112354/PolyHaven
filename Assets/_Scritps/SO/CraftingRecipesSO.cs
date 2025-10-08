using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "ScriptableObjects/Crafting Recipes")]
public class CraftingRecipesSO : ScriptableObject
{
    [Header("----------Tool Recipes----------")]
    public List<CraftingRecipe> toolRecipes;
    [Header("----------Prop Recipes----------")]
    public List<CraftingRecipe> propRecipes;
    [Header("----------House Recipes----------")]   
    public List<CraftingRecipe> houseRecipes;
}

[System.Serializable]
public class CraftingRecipe
{
    public List<Ingredient> ingredients;
    // public bool isOwned;
    public CraftableSO result;
    public int resultAmount = 1;
}

[System.Serializable]
public class Ingredient {
    public MaterialSO item;
    public int quantity;
}