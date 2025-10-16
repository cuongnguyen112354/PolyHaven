using UnityEngine;
using System.Collections.Generic;

public abstract class CraftableSO : ItemSO
{
    [Header("Ingrediens")]
    public List<Ingredient> ingredients;
}
