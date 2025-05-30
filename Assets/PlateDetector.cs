using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlateDetector : MonoBehaviour
{
    public PhotoShotManager photoTaker; // Assign this in the Inspector
    public string uploadUrl = "https://your-api-endpoint.com/upload"; // Replace with your actual API URL

    private HashSet<string> sentFilenames = new HashSet<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Order"))
        {
            if (photoTaker != null)
            {
                string filename = null;

                // Try to get TMP UGUI first
                TextMeshProUGUI tmp = other.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    (int tableId, int foodId, int userId) = ExtractIds(tmp.text);
                    Debug.Log("Plate Text (UI TMP): " + tmp.text);
                    filename = $"order_{foodId}_{userId}_{tableId}";
                }
                else
                {
                    // Fallback: Try 3D TMP
                    TextMeshPro tmp3D = other.GetComponentInChildren<TextMeshPro>();
                    if (tmp3D != null)
                    {
                        (int tableId, int foodId, int userId) = ExtractIds(tmp3D.text);
                        filename = $"order_{foodId}_{userId}_{tableId}";
                    }
                    else
                    {
                        Debug.LogWarning("No TMP or TMPUGUI text found under Plate.");
                    }
                }

                // If filename was successfully parsed and not already sent
                if (!string.IsNullOrEmpty(filename) && !sentFilenames.Contains(filename))
                {
                    sentFilenames.Add(filename);
                    StartCoroutine(photoTaker.TakePhotoAndUpload(uploadUrl, filename));
                }
            }
            else
            {
                Debug.LogWarning("PhotoTaker reference not set!");
            }
        }
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