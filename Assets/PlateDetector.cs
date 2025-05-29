using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlateDetector : MonoBehaviour
{
    public PhotoShotManager photoTaker; // Assign this in the Inspector
    public string uploadUrl = "https://your-api-endpoint.com/upload"; // Replace with your actual API URL
    private bool is_send = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plate") && !is_send)
        {
            if (photoTaker != null)
            {
                // Attempt to find the TMP text in the child hierarchy
                TextMeshProUGUI tmp = other.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp == null)
                {
                    // Fallback: maybe it uses TextMeshPro (3D version) instead of UGUI
                    TextMeshPro tmp3D = other.GetComponentInChildren<TextMeshPro>();
                    if (tmp3D != null)
                    {
                        (int tableId, int foodId, int userId) = ExtractIds(tmp3D.text);
                        string filename = $"order_{foodId}_{userId}_{tableId}";
                        StartCoroutine(photoTaker.TakePhotoAndUpload(uploadUrl, filename));
                    }
                    else
                    {
                        Debug.LogWarning("No TMP or TMPUGUI text found under Plate.");
                    }
                }
                else
                {
                    (int tableId, int foodId, int userId) = ExtractIds(tmp.text);
                    Debug.Log("Plate Text (UI TMP): " + tmp.text);
                    string filename = $"order_{foodId}_{userId}_{tableId}";
                    StartCoroutine(photoTaker.TakePhotoAndUpload(uploadUrl, filename));
                }
                
            }
            else
            {
                Debug.LogWarning("PhotoTaker reference not set!");
            }
            is_send = true;
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