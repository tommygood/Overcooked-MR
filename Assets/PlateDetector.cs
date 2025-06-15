using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class PlateDetector : MonoBehaviour
{
    [SerializeField]
    private GameObject deliveryPrefab;

    public PhotoShotManager photoTaker; // Assign this in the Inspector
    public string uploadUrl = "https://your-api-endpoint.com/upload"; // Replace with your actual API URL

    public string autoGradeUrl = "https://mixed-restaurant.bogay.me/api/cartItem/auto-grade"; // Replace with your actual API URL

    private HashSet<string> sentFilenames = new HashSet<string>();

    public float cleanDelay = 5f; // Delay before cleaning the plate

    private GameObject[] on_plate_ingredients;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[PlateDetector] OnTriggerEnter: " + other.name);

        if (other.CompareTag("Order"))
        {
            SoundManager.Instance.PlaySFX(SoundRegistry.SoundID.BowlCleaned, transform.position);
            if (photoTaker != null)
            {
                string filename = null;

                // Try to get TMP UGUI first
                int food_id = -1;
                int table_id = -1;
                int user_id = -1;
                TextMeshProUGUI tmp = other.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    (int tableId, int foodId, int userId) = ExtractIds(tmp.text);
                    Debug.Log("Plate Text (UI TMP): " + tmp.text);
                    filename = $"order_{foodId}_{userId}_{tableId}";
                    food_id = foodId;
                    table_id = tableId;
                    user_id = userId;
                }
                else
                {
                    // Fallback: Try 3D TMP
                    TextMeshPro tmp3D = other.GetComponentInChildren<TextMeshPro>();
                    if (tmp3D != null)
                    {
                        (int tableId, int foodId, int userId) = ExtractIds(tmp3D.text);
                        filename = $"order_{foodId}_{userId}_{tableId}";
                        food_id = foodId;
                        table_id = tableId;
                        user_id = userId;
                    }
                    else
                    {
                        Debug.LogWarning("No TMP or TMPUGUI text found under Plate.");
                    }
                }

                if (food_id == -1 || table_id == -1 || user_id == -1)
                {
                    Debug.LogWarning("Failed to extract one of ID from the text!");
                    return;
                }

                // If filename was successfully parsed and not already sent
                if (!string.IsNullOrEmpty(filename) && !sentFilenames.Contains(filename))
                {
                    sentFilenames.Add(filename);
                    StartCoroutine(photoTaker.TakePhotoAndUpload(uploadUrl, filename));
                    // clean the plate after few seconds
                    GameObject nearestPlate = FindNearestPlate();
                    if (nearestPlate == null)
                    {
                        Debug.LogWarning("No plate found nearby!");
                        return;
                    }

                    // Check if the plate has a correct combination of ingredients
                    PlateController plate_controller = nearestPlate.GetComponent<PlateController>();
                    Debug.Log("最近的盤子：" + nearestPlate.name);
                    Order top = FindTopIngredientOnPlate(nearestPlate.transform);
                    if (top == null)
                    {
                        Debug.LogWarning("No top ingredient found on the plate!");
                        return;
                    }
                    bool isCorrect = plate_controller.CheckRecipeFromTop(top, food_id);
                    float thisCleanDelay = cleanDelay;
                    float thisDeactivateDelay = 5f;

                    if(this.isTakeout(table_id))
                    {
                        thisCleanDelay = 0f;
                        thisDeactivateDelay = 0f;
                        Instantiate(
                            deliveryPrefab,
                            nearestPlate.transform.position + Vector3.up * 0.5f,
                            nearestPlate.transform.rotation);
                    }

                    if (isCorrect)
                    {
                        Debug.Log("[PlateDetector] 組合正確！");
                        StartCoroutine(SendAutoGradeRequest(autoGradeUrl, food_id.ToString(), user_id.ToString(), table_id.ToString(), "1"));
                    }
                    else
                    {
                        Debug.Log("[PlateDetector] 組合錯誤！");
                    }

                    StartCoroutine(CleanPlateAfterDelay(nearestPlate, thisCleanDelay));
                    StartCoroutine(DeactivateIngredientsAfterDelay(thisDeactivateDelay));
                }
            }
            else
            {
                Debug.LogWarning("PhotoTaker reference not set!");
            }
        }
    }

    private bool isTakeout(int tableId) => tableId > 2;

    private IEnumerator SendAutoGradeRequest(string url, string foodId, string userId, string tableId, string autoGrade, System.Action<string> onSuccess = null, System.Action<string> onError = null)
    {
        UnityWebRequest request = UnityWebRequest.Put(url, ""); // Body can be empty since you're sending data via headers

        // Set custom headers
        request.SetRequestHeader("auto-grade", autoGrade);
        request.SetRequestHeader("food-id", foodId);
        request.SetRequestHeader("user-id", userId);
        request.SetRequestHeader("table-id", tableId);

        // Send the request and wait for response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("PUT request succeeded: " + request.downloadHandler.text);
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("PUT request failed: " + request.error);
            onError?.Invoke(request.error);
        }
    }

    private Order FindTopIngredientOnPlate(Transform plateTransform)
    {
        Vector3 center = plateTransform.position + Vector3.up * 0.5f;
        Collider[] colliders = Physics.OverlapBox(center, new Vector3(0.5f, 1f, 0.5f));

        Order top = null;
        float highestY = float.MinValue;
        List<GameObject> topIngredients = new List<GameObject>();

        foreach (Collider col in colliders)
        {
            Order ord = col.GetComponent<Order>();
            if (ord != null)
            {
                float y = col.transform.position.y;
                if (y > highestY)
                {
                    highestY = y;
                    top = ord;
                }
            }

            if (col.gameObject != null)
            {
                topIngredients.Add(col.gameObject);
            }
        }
        on_plate_ingredients = topIngredients.ToArray();

        return top;
    }

    private IEnumerator DeactivateIngredientsAfterDelay(float delay)
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(delay);

        // Set each GameObject in the ingredients array to inactive
        foreach (GameObject ingredient in on_plate_ingredients)
        {
            if (ingredient != null)
            {
                if (ingredient.tag != "Plate")
                ingredient.SetActive(false);
            }
        }
    }

    private IEnumerator CleanPlateAfterDelay(GameObject plate, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (plate != null)
        {
            // Assuming the plate has a method to clean itself
            WashPlate plateController = plate.GetComponent<WashPlate>();
            if (plateController != null)
            {
                plateController.IsClean = false; // Set to dirty first
                plateController.CleanProgress = 0; // Reset clean progress
            }
            else
            {
                Debug.LogWarning("PlateController not found on the plate!");
            }
        }
        else
        {
            Debug.LogWarning("Plate is null, cannot clean!");
        }
    }

    private GameObject FindNearestPlate()
    {
        // find the nearest plate to the self
        GameObject[] plates = GameObject.FindGameObjectsWithTag("Plate");
        GameObject nearestPlate = null;
        float minDistance = float.MaxValue;
        foreach (GameObject plate in plates)
        {
            float distance = Vector3.Distance(transform.position, plate.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlate = plate;
            }
        }
        return nearestPlate;
    }

    private (int, int, int) ExtractIds(string rawText)
    {
        // Example input: "Neww Order: table=1, food=1, user=1"
        int tableId = -1, foodId = -1, userId = -1;

        string[] parts = rawText.Split(',');
        foreach (string part in parts)
        {
            if (part.Contains("table="))
            {
                int.TryParse(part.Split('=')[1].Trim(), out tableId);
            }
            else if (part.Contains("food="))
            {
                int.TryParse(part.Split('=')[1].Trim(), out foodId);
            }
            else if (part.Contains("user="))
            {
                int.TryParse(part.Split('=')[1].Trim(), out userId);
            }
        }

        Debug.Log($"Extracted IDs -> Table: {tableId}, Food: {foodId}, User: {userId}");
        // Return the extracted values as a tuple
        return (tableId, foodId, userId);
    }
}