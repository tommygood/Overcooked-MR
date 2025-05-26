using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    // 預設食譜，使用 tag 判斷
    private List<string> hamburgerRecipe = new List<string> { "Bread", "Cut_Tomato", "Cheese", "CutLettuce", "Burger_Meat" };
    private List<string> sandwichRecipe = new List<string> { "Chicken", "Toast", "Cheese" };

    public void CheckRecipeFromTop(Order topIngredient)
    {
        List<string> currentOrder = new List<string>();

        Order current = topIngredient;
        while (current != null)
        {
            currentOrder.Add(current.tag);
            current = current.belowIngredient;
        }

        // 注意：我們是從上往下加進 list，要反轉順序以符合配方順序
        currentOrder.Reverse();

        if (MatchRecipe(currentOrder, hamburgerRecipe))
        {
            Debug.Log("✅ 完整漢堡！+100分");
        }
        else if (MatchRecipe(currentOrder, sandwichRecipe))
        {
            Debug.Log("✅ 完整三明治！+100分");
        }
        else
        {
            Debug.Log("❌ 食譜錯誤！0分");
        }
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
