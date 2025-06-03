using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    private Dictionary<int, List<string>> recipes = new Dictionary<int, List<string>>
    {
        { 51, new List<string> { "Bread", "Tomato_cut", "Cheese", "Lettuce_cut", "Cooked_Burger_Meat", "Bread" } }, // hamburger
        { 52, new List<string> { "Toast", "Cheese" ,"Cooked_Tuekeycut","Toast"} },                                   // sandwich
        { 55, new List<string> { "Tortilla", "Cooked_Steakcut", "Lettuce_cut"} },                            // Taco
        { 50, new List<string> { "Toast", "Cheese" } }                           // Sushi
    };

    public bool CheckRecipeFromTop(Order topIngredient, int foodId)
{
    if (topIngredient == null)
    {
        Debug.Log("頂部食材為 null");
        return false;
    }

    // 單層
    Dictionary<int, string> singleItemRecipes = new Dictionary<int, string>
    {
        { 53, "Pumpkin_Soup" },
        { 54, "Carrot_Soup" },
        { 56, "Apple_cut" },
        { 57, "Salad" },
        { 58, "Cooked_Steak" }
    };

    if (singleItemRecipes.ContainsKey(foodId))
    {
        string expectedTag = singleItemRecipes[foodId];
        string actualTag = topIngredient.tag;

        if (actualTag == expectedTag)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 多層
    if (!recipes.ContainsKey(foodId))
    {
        return false;
    }

    List<string> currentOrder = new List<string>();
    Order current = topIngredient;

    while (current != null)
    {
        currentOrder.Add(current.tag);
        current = current.belowIngredient;
    }

    currentOrder.Reverse(); // 由下往上

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
        if (current.Count != recipe.Count) return false;
        for (int i = 0; i < recipe.Count; i++)
        {
            if (current[i] != recipe[i])
                return false;
        }
        return true;
    }
}
