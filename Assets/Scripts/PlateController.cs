using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    private Dictionary<int, List<string>> recipes = new Dictionary<int, List<string>>
    {
        { 51, new List<string> { "Bread", "Tomato_cut", "Cheese", "Lettuce_cut", "Bread" } }, // hamburger
        { 52, new List<string> { "Toast", "Cheese", "Toast"} },                                   // sandwich
        { 55, new List<string> { "Tortilla", "Cooked_Steakcut", "Lettuce_cut"} },                            // Taco
        { 57, new List<string> { "Lettuce_cut", "Tomato_cut" }}
    };

    public bool CheckRecipeFromTop(Order topIngredient, int foodId, GameObject[] on_plate_ingredients)
{
    if (topIngredient == null)
    {
        Debug.Log("頂部食材為 null");
        return false;
    } else
        {
            Debug.Log("頂部食材為" + topIngredient.name);
        }

    // 單層
    Dictionary<int, string> singleItemRecipes = new Dictionary<int, string>
    {
        { 53, "Pumpkin_Soup" },
        { 54, "Carrot_Soup" },
        { 56, "Apple_cut" },
        { 58, "Cooked_Steak" }
    };

    if (singleItemRecipes.ContainsKey(foodId))
        {
            string expectedTag = singleItemRecipes[foodId];
            string actualTag = topIngredient.tag;
            Debug.Log($"檢查單層食物 ID {foodId}： {expectedTag} == {actualTag}");
            return actualTag == expectedTag;
    }

    // 多層
    if (!recipes.ContainsKey(foodId))
    {
        return false;
    }

    List<string> currentOrder = new List<string>();

    foreach (GameObject ingredient in on_plate_ingredients)
    {
        if (ingredient != null)
        {
            currentOrder.Add(ingredient.tag);
        }
    }

    Debug.Log($"檢查多層食物 ID {foodId} 的堆疊順序：");
    for (int i = 0; i < currentOrder.Count; i++)
    {
        Debug.Log($"  {i + 1}. {currentOrder[i]}");
    }

    bool match = MatchRecipe(currentOrder, recipes[foodId]);
    if (match)
    {
        Debug.Log($"符合！(ID={foodId})");
    }
    else
    {
        Debug.Log($"不符合！(ID={foodId})");
    }

    return match;
}



    private bool MatchRecipe(List<string> current, List<string> recipe)
    {
        if (current.Count != recipe.Count)
        {
            Debug.Log("[Food Order Not Matched]: the food number not matched. Expected to be " + recipe.Count + ", but found " + current.Count);
            return false;
        }
        for (int i = 0; i < recipe.Count; i++)
        {
            if (current[i] != recipe[i])
            {
                Debug.Log("[Food Order Not Matched]: the food object not matched. Expected to be " + recipe[i] + ", but found " + current[i]);
                return false;
            }
        }
        return true;
    }
}
