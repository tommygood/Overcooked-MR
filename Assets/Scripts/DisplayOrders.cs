using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class DisplayOrders : MonoBehaviour
{
  public RectTransform contentParent;         // Scroll View Content
  public GameObject textItemPrefab;           // Text prefab (TextMeshProUGUI)
  public float fontSize = 5f;

  public float lineHeight = 1f;
  private float y = 0f;

  [System.Serializable]
  private class Order {
    public int user_id;
    public int food_id;
    public int item_qty;
  }

  private Order[] orders;

  public string api_base_url = "https://163.22.17.116:8002"; // Base URL for API

  public string api_get_items = "/api/cartItem"; // Endpoint for getting orders

  public string api_get_food_list = "/api/foods"; // Endpoint for getting food list

  // store the id and name of the foods
  [System.Serializable]
  private class Food
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

  private Food[] foodList;

  public float check_order_interval = 5f; // Interval to check for new orders
  private float lastCheckTime = 0f;

  void Start()
  {
    StartCoroutine(FetchAndGenerateFoodList());
  }

  void Update()
  {
    // Check if it's time to fetch new orders
    if (lastCheckTime >= check_order_interval)
    {
      lastCheckTime = 0f;
      StartCoroutine(FetchAndGenerateList());
    }
    else
    {
      lastCheckTime += Time.deltaTime;
    }
  }

  private IEnumerator FetchAndGenerateFoodList()
  {
    Debug.Log("Fetching food lists from API...");
    yield return StartCoroutine(GetFoodList());
    if (foodList != null)
    {
      StartCoroutine(FetchAndGenerateList());
    }
  }

  private IEnumerator GetFoodList() {
    string url = api_base_url + api_get_food_list;
    Debug.Log("Fetching food lists from: " + url);
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
      // set the header "x-api-key: 283b65a5e93dc42e58d23b1262cc821226396b71a7fc0f1a1208caaed6d0941f"
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
        // 假設回傳內容為 ["Apple","Banana"]
        Debug.Log("Received food lists: " + request.downloadHandler.text);
        string json = request.downloadHandler.text;
        // set the foodList to the result
        foodList = JsonHelper.FromJson<Food>(json);
      }
    }
  }
  private IEnumerator FetchAndGenerateList()
  {
    Debug.Log("Fetching items from API...");
    yield return StartCoroutine(GetOrders());

    if (orders != null)
    {
      GenerateList();
    }
    else {
      Debug.LogError("Failed to fetch orders.");
    }
  }

  // return a string array of items
  private IEnumerator GetOrders()
  {
    string url = api_base_url + api_get_items;
    Debug.Log("Fetching items from: " + url);
    using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
    {
      // set the header "x-api-key: 283b65a5e93dc42e58d23b1262cc821226396b71a7fc0f1a1208caaed6d0941f"
      request.SetRequestHeader("x-api-key", "283b65a5e93dc42e58d23b1262cc821226396b71a7fc0f1a1208caaed6d0941f");
      request.SetRequestHeader("Content-Type", "application/json");
      request.certificateHandler = new BypassCertificate();
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("Failed to fetch items: " + request.error);
      }
      else
      {
        // 假設回傳內容為 ["Apple","Banana"]
        Debug.Log("Received items: " + request.downloadHandler.text);
        string json = request.downloadHandler.text;
        // set the orders to the result
        orders = JsonHelper.FromJson<Order>(json);
        // check if the orders is null
        if (orders == null)
        {
          Debug.LogError("Failed to parse orders.");
          yield break;
        }
        else
        {
          Debug.Log("Parsed orders: " + orders.Length);
          foreach (Order order in orders)
          {
            Debug.Log("User ID: " + order.user_id + ", Food ID: " + order.food_id + ", Quantity: " + order.item_qty);
          }
        }
      }
    }
  }

  public void GenerateList()
  {
    foreach (Transform child in contentParent)
    {
      Destroy(child.gameObject);
    }

    Debug.Log("Generating list...");
    // show the orders
    foreach (Order order in orders)
    {
      // get the food name from the foodList
      string foodName = "";
      Debug.Log("User ID" + order.user_id + ", Food ID: " + order.food_id + ", Quantity: " + order.item_qty);
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
        textComponent.text = "User ID: " + order.user_id + " Food Name: " + foodName + " Quantity: " + order.item_qty + " \n";
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
}

class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // 跳過憑證驗證（不安全）
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}