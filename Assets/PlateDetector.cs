using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                StartCoroutine(photoTaker.TakePhotoAndUpload(uploadUrl));
            }
            else
            {
                Debug.LogWarning("PhotoTaker reference not set!");
            }
            is_send = true;
        }
    }
}