using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using static OrderController;

public class OrderButtonController : MonoBehaviour
{
    TextMeshPro text_mesh;
    public GameObject PopupMenu;
    public OrderController orderController;
    public GameObject OrderPanel;
    public GameObject PopupMenuCancelButton;

    private void Start()
    {
        text_mesh = GetComponent<TextMeshPro>();
    }

    public static OrderController.Order OrderParser(string input)
    {
        List<int> numbers = new List<int>();

        // Use Regex to find continuous digit sequences
        MatchCollection matches = Regex.Matches(input, @"\d+");

        foreach (Match match in matches)
        {
            if (int.TryParse(match.Value, out int value))
            {
                numbers.Add(value);
            }
        }
        OrderController.Order orders = new OrderController.Order();
        orders.user_id = numbers.ToArray()[0];
        orders.food_id = numbers.ToArray()[1];
        return orders;
    }

    private string getFoodInfo(int food_id)
    {
        string food_info = string.Empty;
        foreach (Food food in orderController.foodList)
        {
            if (food.food_id == food_id)
            {
                food_info = food.food_name;
            }
        }
        return food_info;
    }

        private void OnTriggerEnter(Collider other)
    {
        if (text_mesh != null)
        {
            if (other.gameObject.name.Contains("IndexTip"))
            {
                TMP_Text textComponent = PopupMenu.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    OrderController.Order orders = OrderParser(text_mesh.text);
                    textComponent.text = "Receipt: " + getFoodInfo(orders.food_id);
                    
                    Debug.Log("QQ: " + PopupMenu.activeSelf);
                    PopupMenu.SetActive(true);
                    Debug.Log("LL: " + PopupMenu.activeSelf);
                    OrderPanel.SetActive(false);
                    PopupMenuCancelButton.SetActive(true);
                }
                else
                {
                    Debug.LogError("Failed to find the text component in the PopupMenu");
                }
            }
        }
        else
        {
            Debug.LogError("No TextMeshPro component found on this GameObject.");
        }
    }
}
