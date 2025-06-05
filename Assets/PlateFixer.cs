using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateFixer : MonoBehaviour
{
    // Define the allowed tags
    private HashSet<string> allowedTags = new HashSet<string> { "Cheese", "Toast", "Cooked_Steak", "Bread" };

    // Correct Unity method signature (note: capital 'T' in OnTriggerEnter)
    void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a tag in the allowed list
        if (allowedTags.Contains(other.tag))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.isKinematic = true;
                Debug.Log($"Set {other.name}'s Rigidbody to isKinematic because tag '{other.tag}' matched.");
            }
        }
    }
}
