using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    // 食譜定義（由下往上）
    private List<string> hamburgerRecipe = new List<string> { "Bread", "Cut_Tomato", "Cheese", "CutLettuce", "Burger_Meat","Bread" };
    private List<string> sandwichRecipe = new List<string> {  "Toast", "Cheese"};
    private List<string> tacoRecipe = new List<string> {  "Tortilla", "CutLettuce","Chicken", "Toast"};

    public void CheckRecipeFromTop(Order topIngredient)
    {
        if (topIngredient == null)
        {
            Debug.Log("未放置任何食材！");
            return;
        }

        List<string> currentOrder = new List<string>();
        Order current = topIngredient;

        // 從上往下蒐集堆疊順序
        while (current != null)
        {
            currentOrder.Add(current.tag);
            current = current.belowIngredient;
        }

        currentOrder.Reverse(); // 由下往上與食譜比較

        Debug.Log("堆疊順序（由下至上）:");
        for (int i = 0; i < currentOrder.Count; i++)
        {
            Debug.Log($"  {i + 1}. {currentOrder[i]}");
        }

        if (MatchRecipe(currentOrder, hamburgerRecipe))
        {
            Debug.Log("完整漢堡！+100分");
        }
        else if (MatchRecipe(currentOrder, sandwichRecipe))
        {
            Debug.Log("完整三明治！+100分");
        }
        else
        {
            Debug.Log("食譜錯誤！0分");
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
