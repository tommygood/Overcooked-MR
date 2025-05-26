using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlateController plate;
    public Order currentTopIngredient;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnSubmitDish();
        }
    }

    public void OnSubmitDish()
    {
        if (currentTopIngredient != null)
        {
            plate.CheckRecipeFromTop(currentTopIngredient);
        }
        else
        {
            Debug.Log("❗ 尚未放置食材");
        }
    }
}
