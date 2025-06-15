using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;

public class IngredientGrabController : MonoBehaviour
{
    private void Start()
    {
        // runner = FindObjectOfType<NetworkRunner>(); // Make sure to assign the runner
        // if (runner == null)
        // {
        //     Debug.LogError("NetworkRunner not found in the scene!");
        // }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.name == "TipCollider" || other.name == "FingerTrackingHandCollider")
    //     {
    //         Debug.Log(gameObject.name + " is touched!");

    //         if (runner != null)
    //         {
    //             var newObject = runner.Spawn(prefab);
    //             newObject.transform.position = transform.position;
    //             newObject.transform.rotation = transform.rotation;
    //             Vector3 size = gameObject.GetComponent<Renderer>().bounds.size;
    //             Vector3 offset = new Vector3(0, size.y, 0);
    //             newObject.transform.position += offset;
    //         }
    //         else
    //         {
    //             Debug.LogError("Runner is not initialized.");
    //         }
    //     }
    // }
}
