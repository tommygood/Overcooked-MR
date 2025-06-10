using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using Fusion;

public class IngredientGrabController : MonoBehaviour
{
    public NetworkPrefabRef prefab;

    private NetworkRunner runner;

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>(); // Make sure to assign the runner
        if (runner == null)
        {
            Debug.LogError("NetworkRunner not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "TipCollider" || other.name == "FingerTrackingHandCollider")
        {
            Debug.Log(gameObject.name + " is touched!");

            if (runner != null)
            {
                runner.Spawn(prefab);
            }
            else
            {
                Debug.LogError("Runner is not initialized.");
            }
        }
    }
}
