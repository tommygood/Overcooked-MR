using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Linq; // Required for FirstOrDefault


public class OrderController : MonoBehaviour
{
  public RectTransform contentParent;         // Scroll View Content
  public GameObject textItemPrefab;           // Text prefab (TextMeshProUGUI)
  public GameObject casher;
  public float fontSize = 5f;
  public GameObject OrderObject; // Assign in inspector
  public Transform casherTransform;


  public float lineHeight = 1f;
  private float y = 0f;

  [System.Serializable]
  public class FeedbackResponse
  {
    public FeedbackData[] data;
  }

  [System.Serializable]
  public class FeedbackData
  {
    public int cart_id;
    public int user_id;
    public int food_id;
    public int item_qty;
    public int table_id;
    public string feedback;
    public int grade;
    public int delivered;
    public string food_name;
    public string food_price;
    public string food_discount;
    public string food_src;
    public string user_name;
  }

  [System.Serializable]
  public class Order
  {
    public int table_id;
    public int user_id;
    public int food_id;
    public int item_qty;
    
    public int cart_id;
  }

  public Order[] orders;

  public string api_base_url = "https://163.22.17.116:8002"; // Base URL for API

  public string api_get_items = "/api/cartItem"; // Endpoint for getting orders

  public string api_get_food_list = "/api/foods"; // Endpoint for getting food list

  public string api_feedback = "/api/delivered-items"; // Endpoint for feedback

  private string x_api_key = "283b65a5e93dc42e58d23b1262cc821226396b71a7fc0f1a1208caaed6d0941f"; // API key

  // store the id and name of the foods
  [System.Serializable]
  public class Food
  {
    public int food_id;
    public string food_name;
    public string food_star;
    public string food_vote;
    public string food_price;
    public string food_discount;
    public string food_desc;
    public string food_status;
    public string food_type;
    public string food_category;
    public string food_src;
  }

  public Food[] foodList;

  public float check_order_interval = 5f; // Interval to check for new orders
  private float lastCheckTime = 0f;

  private bool order_not_change = false;

  void Start()
  {
    initOrderPanel();
        
    StartCoroutine(FetchAndGenerateFoodList());
    StartCoroutine(AutoGenerateOrder());
    }

  void Update()
  {
    // Check if it's time to fetch new orders
    if (lastCheckTime >= check_order_interval)
    {
      lastCheckTime = 0f;
      StartCoroutine(FetchAndGenerateList());
      StartCoroutine(DisplayOrderFeedback());
    }
    else
    {
      lastCheckTime += Time.deltaTime;
    }
  }

    private IEnumerator AutoGenerateOrder()
    {
        string url = "https://mixed-restaurant.bogay.me/api/cart/auto-generate";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            // Optionally set headers here, for example:
            // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Order auto-generated successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to auto-generate order: " + request.error);
            }
        }
    }

    private void initOrderPanel()
    {
        if (casher != null)
        {
            // Move this panel to the casher's position
            transform.position = casher.transform.position;

            // Optional offset so it doesn't overlap exactly
            transform.position += new Vector3(0.271f, 3.2f, -0.005f); // e.g., 2 units above the casher
        }
        else
        {
            Debug.LogError("Casher GameObject not assigned.");
        }
    }

  private IEnumerator FetchAndGenerateFoodList()
  {
    yield return StartCoroutine(GetFoodList());
    if (foodList != null && !order_not_change)
    {
      StartCoroutine(FetchAndGenerateList());
    }
  }

  private IEnumerator GetFoodList() {
    string url = api_base_url + api_get_food_list;
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
      request.SetRequestHeader("Content-Type", "application/json");
      request.certificateHandler = new BypassCertificate();
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Failed to fetch food lists: " + request.error);
        yield break;
      }
      else
      {
        string json = request.downloadHandler.text;
        // set the foodList to the result
        foodList = JsonHelper.FromJson<Food>(json);
      }
    }
  }
  private IEnumerator FetchAndGenerateList()
  {
    yield return StartCoroutine(GetOrders());

    if (orders != null)
    {
        if (!order_not_change)
        {
            GenerateList();
        }
    }
    else {
      Debug.LogError("Failed to fetch orders.");
    }
  }

  // return a string array of items
  private IEnumerator GetOrders()
  {
    string url = api_base_url + api_get_items;
    using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
    {
      request.SetRequestHeader("x-api-key", x_api_key);
      request.SetRequestHeader("Content-Type", "application/json");
      request.SetRequestHeader("delivered", "false");
      request.certificateHandler = new BypassCertificate();
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Failed to fetch items: " + request.error);
      }
      else
      {
        string json = request.downloadHandler.text;
        // set the orders to the result
        Order[] orders_temp = JsonHelper.FromJson<Order>(json);
        // check if the orders is null
        if (orders_temp == null)
        {
          Debug.LogError("Failed to parse orders.");
          yield break;
        }
        else if (AreOrdersEqual(orders, orders_temp))
        {
          order_not_change = true;         
        }
        else
        {
          orders = orders_temp;
          order_not_change = false;
        }
      }
    }
  }

    public bool AreOrdersEqual(Order[] a, Order[] b)
    {
        bool is_equal = true;

        if (a == null || b == null) return is_equal;

        foreach (Order orderB in b)
        {
            bool found = false;
            foreach (Order orderA in a)
            {
                if (orderA.user_id == orderB.user_id && orderA.food_id == orderB.food_id)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.Log($"Order not found in A: user_id={orderB.user_id}, food_id={orderB.food_id}");
                is_equal = false;
                GameObject newOrderGO = Instantiate(OrderObject, casherTransform);
                newOrderGO.SetActive(true);
                OrderAnimator orderAnimator = newOrderGO.GetComponent<OrderAnimator>();
                // get the food name from the foodList
                string foodName = "";
                string foodDesc = "";
                foreach (Food food in foodList)
                {
                    if (food.food_id == orderB.food_id)
                    {
                        foodName = food.food_name;
                        foodDesc = food.food_desc;
                        break;
                    }
                }
                orderAnimator.DisplayText($"{foodName}\n{foodDesc}\n\nOrder Info:\ntable={orderB.table_id}, food={orderB.food_id}, user={orderB.user_id}");
                newOrderGO.name = $"Order_{orderB.cart_id}";
                orderAnimator.Up();
            }
        }

        return is_equal;
    }


    public void GenerateList()
  {
    foreach (Transform child in contentParent)
    {
      Destroy(child.gameObject);
    }

    // show the orders
    foreach (Order order in orders)
    {
      // get the food name from the foodList
      string foodName = "";
      foreach (Food food in foodList)
      {
        if (food.food_id == order.food_id)
        {
          foodName = food.food_name;
          break;
        }
      }

      GameObject newTextItem = Instantiate(textItemPrefab, contentParent);
      TMP_Text textComponent = newTextItem.GetComponent<TMP_Text>();
      if (textComponent != null)
      {
        textComponent.text = "TID:" + order.table_id + "FID:" + order.food_id + "x" + order.item_qty + "\n";
        textComponent.fontSize = fontSize;
        textComponent.enabled = true;
      }

      RectTransform rt = newTextItem.GetComponent<RectTransform>();
      rt.anchoredPosition = new Vector2(0, -y);
      y += lineHeight;
    }
    // Reset y position for next generation
    y = 0f;
  }

  private IEnumerator DisplayOrderFeedback()
  {
    // call the feedback API
    // example return value: {"status":true,"message":"Successfully retrieved delivered items","data":[{"cart_id":12,"user_id":3,"food_id":2,"item_qty":1,"table_id":1,"feedback":"aaaa","grade":1,"delivered":1,"food_name":"shrimp tacos","food_price":"15.00","food_discount":"3.00","food_src":"taco/taco-2.png","user_name":"qq"}]}
    string url = api_base_url + api_feedback;
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
      request.SetRequestHeader("x-api-key", x_api_key);
      request.SetRequestHeader("Content-Type", "application/json");
      request.certificateHandler = new BypassCertificate();
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Failed to fetch feedback: " + request.error);
      }
      else
      {
        string json = request.downloadHandler.text;
        // Parse the JSON and display feedback
        Debug.Log("Feedback: " + json);
        // You can further process the feedback data here
        // extract the feedback and grade from the JSON
        var feedbackData = JsonUtility.FromJson<FeedbackResponse>(json);
        if (feedbackData != null && feedbackData.data != null)
        {
          foreach (var item in feedbackData.data)
          {
            // find the order Gameobject by order's name (Order_cart_id)
            GameObject orderGO = GameObject.Find($"Order_{item.cart_id}");
            if (orderGO != null)
            {
              string text = $"Feedback: {item.feedback}\n Grade: {item.grade}";
              Debug.Log(text);
              TMP_Text textComponent = orderGO.GetComponentsInChildren<TMP_Text>()
              .FirstOrDefault(t => t.gameObject.name == "OrderFeedback");
              if (textComponent == null)
                Debug.LogError("Failed to find the text component in the OrderFeedback");
              else textComponent.text = text;
            }
            else
            {
              Debug.LogWarning($"Failed to fill the feedback, Order with cart_id {item.cart_id} not found in the scene.");
            }
          }
        }
      }
    }
  }

  public IEnumerator DeleteOrder(int user_id, int food_id)
  {
    // Delete order from API
    string url = api_base_url + "/api/cartItem/" + user_id + "/" + food_id;
    //Debug.Log("Deleting order from: " + url);
    using (UnityWebRequest request = UnityWebRequest.Delete(url))
    {
      request.SetRequestHeader("x-api-key", x_api_key);
      request.SetRequestHeader("Content-Type", "application/json");
      request.certificateHandler = new BypassCertificate();
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Failed to delete order: " + request.error);
      }
    }
  }
}

class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"items\":" + json + "}";
        // do the try catch here
        try
        {
          Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
          return wrapper.items;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse JSON: " + e.Message + "\n" + json);
            return new T[0]; // Return an empty array on failure
        }
        
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}